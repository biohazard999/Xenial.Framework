using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xenial.Cli.Engine.Syntax;

public class LayoutBuilderAttributeSyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel model;

    public LayoutBuilderAttributeSyntaxRewriter(SemanticModel model)
        => this.model = model;

    public override SyntaxNode? VisitAttributeList(AttributeListSyntax node)
    {

        return base.VisitAttributeList(node);
    }
}
