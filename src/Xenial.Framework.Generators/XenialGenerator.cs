using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xenial.Framework.Generators;


[Generator]
public class XenialGenerator : ISourceGenerator
{
    public IList<IXenialSourceGenerator> Generators { get; } = new List<IXenialSourceGenerator>
    {
        new XenialImageNamesGenerator(),
        new XenialActionGenerator()
    };

    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<TypeDeclarationSyntax> Types { get; } = new();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclarationSyntax)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

                if (classSymbol is not null)
                {
                    Types.Add(classDeclarationSyntax);
                }
            }
            if (context.Node is RecordDeclarationSyntax { AttributeLists.Count: > 0 } recordDeclarationSyntax)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(recordDeclarationSyntax);

                if (classSymbol is not null)
                {
                    Types.Add(recordDeclarationSyntax);
                }
            }
        }
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver syntaxReceiver)
        {
            return;
        }

        var compilation = context.Compilation;
        foreach (var generator in Generators)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            compilation = generator.Execute(context, compilation, syntaxReceiver.Types);
        }
    }
}

public interface IXenialSourceGenerator
{
    Compilation Execute(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<TypeDeclarationSyntax> types
    );
}
