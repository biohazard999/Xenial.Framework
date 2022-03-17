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

public class LayoutBuilderAttributeSyntaxRewriter : CSharpSyntaxRewriter
{
    private class LayoutBuilderAttributeSyntaxWalker : CSharpSyntaxWalker
    {
        readonly SemanticModel model;
        readonly string builderName;
        public LayoutBuilderAttributeSyntaxWalker(SemanticModel model!!, string builderName!!)
            => (this.model, this.builderName) = (model, builderName);

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
                    var attribute = GetAttribute(classSymbol, attributeSymbol);
                    if (attribute.ConstructorArguments.Length == 1)
                    {
                        var argument = attribute.ConstructorArguments[0];
                        if (argument.Kind == TypedConstantKind.Type)
                        {
                            if (builderName.Equals(argument.Value?.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                ShouldAddAttribute = false;
                                return;
                            }
                        }
                    }
                }
                ShouldAddAttribute = true;
            }

            base.VisitClassDeclaration(node);
        }

        private static bool HasAttribute(
            ISymbol symbol,
            ISymbol attributeSymbol)
            => symbol.GetAttributes()
                       .Any(m => m.AttributeClass?.ToString() == attributeSymbol.ToString());

        private static AttributeData GetAttribute(
            ISymbol property,
            ISymbol attributeSymbol)
            => property.GetAttributes()
               .First(m => m.AttributeClass?.ToString() == attributeSymbol.ToString());


    }

    private readonly SemanticModel model;
    private readonly string builderName;
    private readonly LayoutBuilderAttributeSyntaxWalker walker;

    public string AttributeName { get; set; } = "DetailViewLayoutBuilder";


    public LayoutBuilderAttributeSyntaxRewriter(SemanticModel model!!, string builderName!!)
        => (this.model, this.builderName, walker) = (model, builderName, new(model, builderName));

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
            var attributes = node.AttributeLists.Add(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(AttributeName))
                            .WithArgumentList(SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.AttributeArgument(
                                        SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(builderName))
                                    )
                                )
                            )
                        )
                    )
                ).NormalizeWhitespace());
            return node.WithAttributeLists(attributes);
        }

        return base.VisitClassDeclaration(node);
    }

}
