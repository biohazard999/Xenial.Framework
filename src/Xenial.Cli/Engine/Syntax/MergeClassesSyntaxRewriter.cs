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
        public Stack<UsingDirectiveSyntax> Usings { get; } = new();
        public Stack<AttributeListSyntax> Attributes { get; } = new();

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            Attributes.Push(node);
            base.VisitAttributeList(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            Usings.Push(node);
            base.VisitUsingDirective(node);
        }

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

    public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node!!)
    {
        var newSyntaxTree = CSharpSyntaxTree.ParseText(codeResult.Code, (CSharpParseOptions)semanticModel.SyntaxTree.Options);
        var newWalker = new MethodWalker();
        newWalker.Visit(newSyntaxTree.GetRoot());

        while (newWalker.Usings.TryPop(out var @using) && !node.Usings.Any(u => u.IsEquivalentTo(@using)))
        {
            node = node.WithUsings(node.Usings.Add(@using));
        }

        return base.VisitCompilationUnit(node);
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node!!)
    {
        var newSyntaxTree = CSharpSyntaxTree.ParseText(codeResult.Code, (CSharpParseOptions)semanticModel.SyntaxTree.Options);
        var newWalker = new MethodWalker();
        newWalker.Visit(newSyntaxTree.GetRoot());

        var attributeList = node.AttributeLists;

        while (newWalker.Attributes.TryPop(out var newAttribute))
        {
            var existingAttribute = attributeList.FirstOrDefault(m => m.IsEquivalentTo(newAttribute));
            if (existingAttribute is not null)
            {
                attributeList = attributeList.Replace(existingAttribute, newAttribute);
            }
            else
            {
                attributeList = attributeList.Add(newAttribute);
            }
        }

        node = node.WithAttributeLists(attributeList);

        var methodList = node.Members;

        while (newWalker.Methods.TryPop(out var newMethod))
        {
            var existingMethod = methodList.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.IsEquivalentTo(newMethod.Identifier));
            if (existingMethod is not null)
            {
                var isFirst = existingMethod.IsEquivalentTo(methodList.OfType<MethodDeclarationSyntax>().FirstOrDefault());

                newMethod = isFirst
                    ? newMethod
                    : newMethod.WithLeadingTrivia(SyntaxFactory.CarriageReturn);

                methodList = methodList.Replace(existingMethod, newMethod);
            }
            else
            {
                methodList = methodList.Add(newMethod.WithLeadingTrivia(SyntaxFactory.CarriageReturn));
            }
        }

        node = node.WithMembers(methodList);

        return base.VisitClassDeclaration(node);
    }
}
