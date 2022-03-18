using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using Xenial.Framework.Layouts;

namespace Xenial.Cli.Engine.Syntax;

public record LayoutAttributeInfo(string LayoutBuilderClass!!)
{
    public string? LayoutBuilderMethod { get; set; }
    public string? ViewId { get; set; }
}


public class LayoutBuilderAttributeSyntaxRewriter : CSharpSyntaxRewriter
{
    private class LayoutBuilderAttributeSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly SemanticModel model;
        private readonly LayoutAttributeInfo builderInfo;
        public LayoutBuilderAttributeSyntaxWalker(SemanticModel model!!, LayoutAttributeInfo builderInfo!!)
            => (this.model, this.builderInfo) = (model, builderInfo);

        public bool HasLayoutsUsing { get; private set; }
        public bool HasClass { get; private set; }

        public bool ShouldAddUsing => HasLayoutsUsing && HasClass;
        public bool ShouldAddAttribute { get; private set; }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var usingStrings = node.Usings.Select(s => s.GetText().ToString().Trim()).ToArray();

            if (!usingStrings.Contains("using Xenial.Framework.Layouts;"))
            {
                HasLayoutsUsing = true;
            }
            base.VisitCompilationUnit(node);
        }

        //[DetailViewLayoutBuilder()]
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            HasClass = true;

            var classSymbol = model.GetDeclaredSymbol(node);
            var attributeSymbol = model.Compilation.GetTypeByMetadataName("Xenial.Framework.Layouts.DetailViewLayoutBuilderAttribute");

            if (classSymbol is not null && attributeSymbol is not null)
            {
                if (HasAttribute(classSymbol, attributeSymbol))
                {
                    var attributes = GetAttributes(classSymbol, attributeSymbol);

                    static bool shouldAddAttribute(AttributeData attribute, LayoutAttributeInfo builderInfo)
                        => attribute.ConstructorArguments.Length switch
                        {
                            1 => attribute.ConstructorArguments[0] switch
                            {
                                var argument and { Kind: TypedConstantKind.Type }
                                    => !builderInfo.LayoutBuilderClass.Equals(argument.Value?.ToString(), StringComparison.OrdinalIgnoreCase),
                                _ => true
                            },
                            2 => (attribute.ConstructorArguments[0], attribute.ConstructorArguments[1]) switch
                            {
                                var (classArgument, methodArgument)
                                    => !builderInfo.LayoutBuilderClass.Equals(classArgument.Value?.ToString(), StringComparison.OrdinalIgnoreCase)
                                        && builderInfo.LayoutBuilderMethod is not null
                                        && !builderInfo.LayoutBuilderMethod.Equals(methodArgument.Value?.ToString(), StringComparison.OrdinalIgnoreCase),
                            },
                            _ => true
                        };

                    ShouldAddAttribute = attributes.Any(attribute => shouldAddAttribute(attribute, builderInfo));
                }
                else
                {
                    ShouldAddAttribute = true;
                }
            }

            base.VisitClassDeclaration(node);
        }

        private static bool HasAttribute(
            ISymbol symbol,
            ISymbol attributeSymbol)
            => symbol.GetAttributes()
                       .Any(m => m.AttributeClass?.ToString() == attributeSymbol.ToString());

        private static IEnumerable<AttributeData> GetAttributes(
            ISymbol property,
            ISymbol attributeSymbol)
            => property.GetAttributes()
               .Where(m => m.AttributeClass?.ToString() == attributeSymbol.ToString());


    }

    private readonly SemanticModel model;
    private readonly LayoutAttributeInfo builderInfo;
    private readonly LayoutBuilderAttributeSyntaxWalker walker;

    public string AttributeName { get; set; } = "DetailViewLayoutBuilder";


    public LayoutBuilderAttributeSyntaxRewriter(SemanticModel model!!, LayoutAttributeInfo builderInfo!!)
        => (this.model, this.builderInfo, walker) = (model, builderInfo, new(model, builderInfo));

    [return: NotNullIfNotNull("node")]
    public override SyntaxNode? Visit(SyntaxNode? node)
    {
        walker.Visit(node);
        return base.Visit(node);
    }

    public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node!!)
    {
        if (walker.ShouldAddUsing)
        {
            var @using = SyntaxFactory.UsingDirective(
                SyntaxFactory.IdentifierName("Xenial.Framework.Layouts")
            );

            return node.AddUsings(@using);
        }

        return base.VisitCompilationUnit(node);
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node!!)
    {
        if (walker.ShouldAddAttribute)
        {
            var arguments = new SeparatedSyntaxList<AttributeArgumentSyntax>().Add(SyntaxFactory.AttributeArgument(
                SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(builderInfo.LayoutBuilderClass))
            ));

            arguments = builderInfo.LayoutBuilderMethod switch
            {
                string _ => arguments.Add(SyntaxFactory.AttributeArgument(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(
                            SyntaxFactory.Identifier(
                                SyntaxFactory.TriviaList(),
                                SyntaxKind.NameOfKeyword,
                                "nameof",
                                "nameof",
                                SyntaxFactory.TriviaList())))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(builderInfo.LayoutBuilderClass),
                                        SyntaxFactory.IdentifierName(builderInfo.LayoutBuilderMethod)))))))
                ),
                _ => arguments
            };

            arguments = builderInfo.ViewId switch
            {
                string _ => arguments.Add(SyntaxFactory.AttributeArgument(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(builderInfo.ViewId)))
                    .WithNameEquals(
                        SyntaxFactory.NameEquals(
                            SyntaxFactory.IdentifierName(nameof(DetailViewLayoutBuilderAttribute.ViewId))))
                ),
                _ => arguments
            };

            var attributes = node.AttributeLists.Add(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(AttributeName))
                            .WithArgumentList(SyntaxFactory.AttributeArgumentList(
                                arguments
                            )
                        )
                    )
                ).NormalizeWhitespace());
            return node.WithAttributeLists(attributes);
        }

        return base.VisitClassDeclaration(node);
    }

}
