using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xenial.Cli.Engine.Syntax;

public class InjectXenialLayoutBuilderModuleSyntaxRewriter : CSharpSyntaxRewriter
{
    public Compilation Compilation { get; }

    public bool WasRewritten { get; private set; }

    public IList<ISymbol> RewrittenSymbols { get; } = new List<ISymbol>();

    private readonly Walker walker;

    public InjectXenialLayoutBuilderModuleSyntaxRewriter(Compilation compilation)
        => (Compilation, walker) = (compilation, new(compilation));

    private class Walker : CSharpSyntaxWalker
    {
        public Compilation Compilation { get; }
        public Walker(Compilation compilation)
            => Compilation = compilation;

        public ClassDeclarationSyntax ClassToChange { get; set; }
        public INamedTypeSymbol SymbolToChange { get; set; }
        public MethodDeclarationSyntax? AddGeneratorUpdatersMethod { get; set; }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var semanticModel = Compilation.GetSemanticModel(node.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(node);
            var moduleBaseSymbol = Compilation.GetTypeByMetadataName(typeof(ModuleBase).FullName!);

            static bool HasBase(INamedTypeSymbol? symbol, INamedTypeSymbol? baseType)
            {
                if (symbol is null) { return false; }
                if (baseType is null) { return false; }

                if (symbol.BaseType is null)
                {
                    return false;
                }

                if (symbol.BaseType.ToString() == baseType.ToString())
                {
                    return true;
                }
                return HasBase(symbol.BaseType, baseType);
            }

            if (HasBase(symbol, moduleBaseSymbol) && symbol is not null && !symbol.IsAbstract)
            {
                ClassToChange = node;
                SymbolToChange = symbol;
            }

            base.VisitClassDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.Identifier.ToString() == "AddGeneratorUpdaters")
            {
                AddGeneratorUpdatersMethod = node;
            }
            base.VisitMethodDeclaration(node);
        }
    }

    public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
    {
        WasRewritten = false;
        walker.Visit(node);
        return base.VisitCompilationUnit(node);
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (node is not null && node.IsEquivalentTo(walker.ClassToChange))
        {
            if (!RewrittenSymbols.Contains(walker.SymbolToChange, SymbolEqualityComparer.Default))
            {
                RewrittenSymbols.Add(walker.SymbolToChange);
                WasRewritten = true;
                if (walker.AddGeneratorUpdatersMethod is not null)
                {
                    var newMethod = walker.AddGeneratorUpdatersMethod.AddBodyStatements(UseXenialStatements().ToArray());
                    return node.ReplaceNode(walker.AddGeneratorUpdatersMethod, newMethod);
                }
                return node.AddMembers(CreateMethod());
            }
        }
        return base.VisitClassDeclaration(node);
    }

    private static MemberDeclarationSyntax CreateMethod() => SyntaxFactory.MethodDeclaration
    (
        SyntaxFactory.PredefinedType
        (
            SyntaxFactory.Token(SyntaxKind.VoidKeyword)
        ),
        SyntaxFactory.Identifier("AddGeneratorUpdaters")
    )
    .WithModifiers
    (
        SyntaxFactory.TokenList
        (
            new[]
            {
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.OverrideKeyword)
            }
        )
    )
    .WithParameterList
    (
        SyntaxFactory.ParameterList
        (
            SyntaxFactory.SingletonSeparatedList<ParameterSyntax>
            (
                SyntaxFactory.Parameter
                (
                    SyntaxFactory.Identifier("updaters")
                )
                .WithType
                (
                    SyntaxFactory.IdentifierName("ModelNodesGeneratorUpdaters")
                )
            )
        )
    )
    .WithBody
    (
        SyntaxFactory.Block
        (
            new SyntaxList<StatementSyntax>(new[]
            {
                SyntaxFactory.ExpressionStatement
                (
                    SyntaxFactory.InvocationExpression
                    (
                        SyntaxFactory.MemberAccessExpression
                        (
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.BaseExpression(),
                            SyntaxFactory.IdentifierName("AddGeneratorUpdaters")
                        )
                    )
                    .WithArgumentList
                    (
                        SyntaxFactory.ArgumentList
                        (
                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>
                            (
                                SyntaxFactory.Argument
                                (
                                    SyntaxFactory.IdentifierName("updaters")
                                )
                            )
                        )
                    )
                ).WithTrailingTrivia(SyntaxFactory.CarriageReturn)
            }.Concat(UseXenialStatements())
            )
        )
    );

    private static IEnumerable<ExpressionStatementSyntax> UseXenialStatements()
    {
        yield return SyntaxFactory.ExpressionStatement
        (
            SyntaxFactory.InvocationExpression
            (
                SyntaxFactory.MemberAccessExpression
                (
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("updaters"),
                    SyntaxFactory.IdentifierName("UseNoViewsGeneratorUpdater")
                )
            )
        );
        yield return SyntaxFactory.ExpressionStatement
        (
            SyntaxFactory.InvocationExpression
            (
                SyntaxFactory.MemberAccessExpression
                (
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("updaters"),
                    SyntaxFactory.IdentifierName("UseDeclareViewsGeneratorUpdater")
                )
            )
        );
        yield return SyntaxFactory.ExpressionStatement
        (
            SyntaxFactory.InvocationExpression
            (
                SyntaxFactory.MemberAccessExpression
                (
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("updaters"),
                    SyntaxFactory.IdentifierName("UseDetailViewLayoutBuilders")
                )
            )
        );
        yield return SyntaxFactory.ExpressionStatement
        (
            SyntaxFactory.InvocationExpression
            (
                SyntaxFactory.MemberAccessExpression
                (
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("updaters"),
                    SyntaxFactory.IdentifierName("UseListViewColumnBuilders")
                )
            )
        );
    }
}
