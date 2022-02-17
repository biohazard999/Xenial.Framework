using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xenial.Framework.Generators.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xenial.Framework.Generators.Internal;
using Xenial.Framework.Generators.Partial;

namespace Xenial.Framework.Generators;

[Generator]
public record XenialGenerator(bool AddSources = true) : ISourceGenerator
{
    public XenialGenerator() : this(true) { }

    private const string xenialDebugSourceGenerators = "XenialDebugSourceGenerators";

    public IList<IXenialSourceGenerator> Generators { get; } = new List<IXenialSourceGenerator>
    {
        //Attributes
        new SystemIsExternalInitAttributeGenerator(AddSources),
        new XenialActionAttributeGenerator(AddSources),
        new XenialExpandMemberAttributeGenerator(AddSources),
        new XenialImageNamesAttributeGenerator(AddSources),
        new XenialViewIdsAttributeGenerator(AddSources),
        new XenialXpoBuilderAttributeGenerator(AddSources),
        new XenialCollectControllersAttributeGenerator(AddSources),
        new XenialCollectExportedTypesAttributeGenerator(AddSources),

        //Generators
        new XenialViewIdsGenerator(AddSources),
        new XenialImageNamesGenerator(AddSources),
        new XenialXpoBuilderGenerator(AddSources),
        new XenialActionGenerator(new(),AddSources),
        new XenialModelBuilderGenerator(AddSources),
        new XenialLayoutBuilderGenerator(AddSources),
        new XenialColumnsBuilderGenerator(AddSources),
        new XenialCollectControllersGenerator(AddSources),
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
                    || new XenialGenerator(false)
                            .Generators
                            .Any(generator => generator.Accepts(typeDeclarationSyntax))
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
                throw;
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
