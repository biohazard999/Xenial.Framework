using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xenial.Framework.DevTools.X2C;

namespace Xenial.Cli.Engine.Syntax;

public class MergeClassesSyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel semanticModel;
    private readonly X2CCodeResult codeResult;
    public MergeClassesSyntaxRewriter(SemanticModel semanticModel, X2CCodeResult codeResult)
        => (this.semanticModel, this.codeResult) = (semanticModel, codeResult);

    private class MethodWalker : CSharpSyntaxWalker
    {
        public Stack<MethodDeclarationSyntax> Methods { get; } = new();

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);

            //We are not interested in nested methods
            if (!TryGetParentSyntax<TypeDeclarationSyntax>(node, out var _))
            {
                Methods.Push(node);
            }
        }

        public static bool TryGetParentSyntax<T>(SyntaxNode? syntaxNode, out T? result)
            where T : SyntaxNode
        {
            // set defaults
            result = null;

            if (syntaxNode == null)
            {
                return false;
            }

            try
            {
                syntaxNode = syntaxNode.Parent;

                if (syntaxNode == null)
                {
                    return false;
                }

                if (syntaxNode.GetType() == typeof(T))
                {
                    result = syntaxNode as T;
                    return true;
                }

                return TryGetParentSyntax(syntaxNode, out result);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node!!)
    {
        var newSyntaxTree = CSharpSyntaxTree.ParseText(codeResult.Code, (CSharpParseOptions)semanticModel.SyntaxTree.Options);
        var newWalker = new MethodWalker();
        newWalker.Visit(newSyntaxTree.GetRoot());

        var methodList = node.Members;

        while (newWalker.Methods.TryPop(out var newMethod))
        {
            methodList = methodList.Add(newMethod.WithLeadingTrivia(SyntaxFactory.CarriageReturn));
        }

        node = node.WithMembers(methodList);

        return base.VisitClassDeclaration(node);
    }
}
