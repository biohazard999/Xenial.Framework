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

public record ColumnsAttributeInfo(string ColumnsBuilderClass!!)
{
    public string? ColumnsBuilderMethod { get; set; }
    public string? ViewId { get; set; }
}


public class ColumnsBuilderAttributeSyntaxRewriter : CSharpSyntaxRewriter
{
    private class ColumnsBuilderAttributeSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly SemanticModel model;
        private readonly ColumnsAttributeInfo builderInfo;
        public ColumnsBuilderAttributeSyntaxWalker(SemanticModel model!!, ColumnsAttributeInfo builderInfo!!)
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

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            HasClass = true;

            var classSymbol = model.GetDeclaredSymbol(node);

            var attributeSymbol = model.Compilation.GetTypeByMetadataName(typeof(ListViewColumnsBuilderAttribute).FullName!);

            if (classSymbol is not null && attributeSymbol is not null)
            {
                if (HasAttribute(classSymbol, attributeSymbol))
                {
                    var attributes = GetAttributes(classSymbol, attributeSymbol);

                    static bool IsAttributeEquivalent(ColumnsAttributeInfo builderInfo, AttributeData data, Compilation compilation)
                    {
                        static TypedConstant? GetAttribute(
                            AttributeData attribute,
                            string attributeName)
                        {
                            var namedArgument = attribute.NamedArguments.FirstOrDefault(argument => argument.Key == attributeName);

                            if (namedArgument.Key == attributeName)
                            {
                                return namedArgument.Value;
                            }

                            return null;
                        }

                        var viewId = GetAttribute(data, nameof(ColumnsBuilderAttribute.ViewId));

                        if (data.ConstructorArguments.Length > 0)
                        {
                            var className = data.ConstructorArguments[0].Value?.ToString();
                            var methodName = data.ConstructorArguments.Length > 1 ?
                                data.ConstructorArguments[1].Value?.ToString()
                                : null;

                            var typeInfo = compilation.GetTypeByMetadataName(className!);

                            //Patch full qualified name.
                            className = typeInfo is null ? className : typeInfo.Name;

                            var attributeInfo = new ColumnsAttributeInfo(className!)
                            {
                                ColumnsBuilderMethod = methodName,
                                ViewId = viewId?.Value?.ToString(),
                            };

                            return builderInfo.Equals(attributeInfo);
                        }

                        return false;
                    }

                    var count = attributes.Count(a => IsAttributeEquivalent(builderInfo, a, model.Compilation));

                    ShouldAddAttribute = count <= 0;
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
    private readonly ColumnsAttributeInfo builderInfo;
    private readonly ColumnsBuilderAttributeSyntaxWalker walker;

    public string AttributeName { get; set; } = "ListViewColumnsBuilder";

    public ColumnsBuilderAttributeSyntaxRewriter(SemanticModel model!!, ColumnsAttributeInfo builderInfo!!)
        => (this.model, this.builderInfo, walker) = (model, builderInfo, new(model, builderInfo));

    public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node!!)
    {
        walker.Visit(node);

        if (walker.ShouldAddUsing)
        {
            var @using = SyntaxFactory.UsingDirective(
                SyntaxFactory.IdentifierName("Xenial.Framework.Layouts")
            );

            node = node.AddUsings(@using);
        }

        return base.VisitCompilationUnit(node);
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node!!)
    {
        if (walker.ShouldAddAttribute)
        {
            var arguments = new SeparatedSyntaxList<AttributeArgumentSyntax>().Add(SyntaxFactory.AttributeArgument(
                SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(builderInfo.ColumnsBuilderClass))
            ));

            arguments = builderInfo.ColumnsBuilderMethod switch
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
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(builderInfo.ColumnsBuilderClass),
                                        SyntaxFactory.IdentifierName(builderInfo.ColumnsBuilderMethod)))))))
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
                            SyntaxFactory.IdentifierName(nameof(ColumnsBuilderAttribute.ViewId))))
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

            node = node.WithAttributeLists(attributes);
        }

        return base.VisitClassDeclaration(node);
    }

}
