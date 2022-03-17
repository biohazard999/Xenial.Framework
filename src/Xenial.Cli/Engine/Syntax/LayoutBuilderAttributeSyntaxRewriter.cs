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

namespace Xenial.Cli.Engine.Syntax;

public class LayoutBuilderAttributeSyntaxRewriter : CSharpSyntaxRewriter
{
    private class LayoutBuilderAttributeSyntaxWalker : CSharpSyntaxWalker
    {
        public bool HasLayoutsUsing { get; private set; }
        public bool HasClass { get; private set; }

        public bool ShouldAddUsing => HasLayoutsUsing && HasClass;

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
            base.VisitClassDeclaration(node);
        }
    }

    private readonly SemanticModel model;
    private readonly string builderName;
    private readonly LayoutBuilderAttributeSyntaxWalker walker = new();

    public string AttributeName { get; set; } = "DetailViewLayoutBuilder";


    public LayoutBuilderAttributeSyntaxRewriter(SemanticModel model!!, string builderName!!)
        => (this.model, this.builderName) = (model, builderName);

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

}
