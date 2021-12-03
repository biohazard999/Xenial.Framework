using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xenial.Framework.Generators.Internal;

namespace Xenial.Framework.Generators;

[Generator]
public class XenialGenerator : ISourceGenerator
{
    private const string xenialDebugSourceGenerators = "XenialDebugSourceGenerators";

    public IList<IXenialSourceGenerator> Generators { get; } = new List<IXenialSourceGenerator>
    {
        new XenialTypeForwardTypesGenerator(),
        new XenialImageNamesGenerator(),
        new XenialActionGenerator()
    };

    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<TypeDeclarationSyntax> Types { get; } = new();

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

        CheckForDebugger(context);

        var compilation = context.Compilation;
        foreach (var generator in Generators)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            compilation = generator.Execute(context, compilation, syntaxReceiver.Types);
        }
    }

    private static void CheckForDebugger(GeneratorExecutionContext context)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{xenialDebugSourceGenerators}", out var xenialDebugSourceGeneratorsAttrString))
        {
            if (bool.TryParse(xenialDebugSourceGeneratorsAttrString, out var xenialDebugSourceGeneratorsBool))
            {
                if (xenialDebugSourceGeneratorsBool)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        return;
                    }

                    System.Diagnostics.Debugger.Launch();
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            xenialDebugSourceGenerators,
                            xenialDebugSourceGeneratorsAttrString
                        )
                        , null
                    ));
            }
        }
    }
}
