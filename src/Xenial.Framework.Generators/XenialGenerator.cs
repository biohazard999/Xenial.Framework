using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        new XenialDuckTypesGenerator(),
        new XenialTypeForwardTypesGenerator(),
        new XenialViewIdsGenerator(),
        new XenialImageNamesGenerator(),
        new XenialXpoBuilderGenerator(),
        new XenialActionGenerator(new()),
        new XenialModelBuilderGenerator(),
        new XenialExpandMemberAttributeGenerator(),
        new XenialLayoutBuilderGenerator()
    };

    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<TypeDeclarationSyntax> Types { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                if (
                    typeDeclarationSyntax.AttributeLists.Count > 0
                    || typeDeclarationSyntax.HasModifier(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)
                    || new XenialGenerator().Generators.Any(generator => generator.Accepts(typeDeclarationSyntax))
                )
                {
                    var classSymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax);

                    if (classSymbol is not null)
                    {
                        Types.Add(typeDeclarationSyntax);
                    }
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

        var addedSourceFiles = new List<string>();
        foreach (var generator in Generators)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
#if DEBUG
            try
            {
#endif
                compilation = generator.Execute(context, compilation, syntaxReceiver.Types, addedSourceFiles);
#if DEBUG
            }
            catch (ArgumentException)
            {
                if (!Debugger.IsAttached)
                {
                    Debugger.Launch();
                }
            }
#endif
        }
    }

    private static void CheckForDebugger(GeneratorExecutionContext context)
    {
        //if (System.Diagnostics.Debugger.IsAttached)
        //{
        //    return;
        //}

        //System.Diagnostics.Debugger.Launch();

        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{xenialDebugSourceGenerators}", out var xenialDebugSourceGeneratorsAttrString))
        {
            if (bool.TryParse(xenialDebugSourceGeneratorsAttrString, out var xenialDebugSourceGeneratorsBool))
            {
                if (xenialDebugSourceGeneratorsBool)
                {
                    if (Debugger.IsAttached)
                    {
                        return;
                    }

                    Debugger.Launch();
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
