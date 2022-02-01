using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators.Internal;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators;

public record XenialExpandMemberAttributeGenerator(bool AddSources = true) : IXenialSourceGenerator
{
    private const string xenialExpandMemberAttributeName = "XenialExpandMemberAttribute";
    private const string xenialNamespace = "Xenial";
    internal const string XenialExpandMemberAttributeFullName = $"{xenialNamespace}.{xenialExpandMemberAttributeName}";
    public const string GenerateXenialXenialExpandMemberAttributeMSBuildProperty = $"Generate{xenialExpandMemberAttributeName}";

    public bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, IList<string> addedSourceFiles)
        => GenerateAttribute(context, compilation, addedSourceFiles);

    private Compilation GenerateAttribute(GeneratorExecutionContext context, Compilation compilation, IList<string> addedSourceFiles)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateXenialXenialExpandMemberAttributeMSBuildProperty}", out var generateXenialViewIdsAttrStr))
        {
            if (bool.TryParse(generateXenialViewIdsAttrStr, out var generateXenialViewIdsAttr))
            {
                if (!generateXenialViewIdsAttr)
                {
                    return compilation;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            GenerateXenialXenialExpandMemberAttributeMSBuildProperty,
                            generateXenialViewIdsAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var (source, syntaxTree) = GenerateXenialLayoutBuilderAttribute(
            (CSharpParseOptions)context.ParseOptions,
            context.GetDefaultAttributeModifier(),
            context.CancellationToken
        );

        if (AddSources)
        {
            var fileName = $"{xenialExpandMemberAttributeName}.g.cs";
            addedSourceFiles.Add(fileName);
            context.AddSource(fileName, source);
        }

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialLayoutBuilderAttribute(
        CSharpParseOptions? parseOptions = null,
        string visibility = "internal",
        CancellationToken cancellationToken = default)
    {
        parseOptions ??= CSharpParseOptions.Default;

        var syntaxWriter = CurlyIndenter.Create();

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine($"using System.ComponentModel;");
        syntaxWriter.WriteLine();

        using (syntaxWriter.OpenBrace($"namespace {xenialNamespace}"))
        {
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]");

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialExpandMemberAttribute : Attribute"))
            {
                syntaxWriter.WriteLine($"public string ExpandMember {{ get; private set; }}");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialExpandMemberAttribute(string expandMember)"))
                {
                    syntaxWriter.WriteLine($"this.ExpandMember = expandMember;");
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }
}
