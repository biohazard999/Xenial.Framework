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

internal class XenialTypeForwardTypesGenerator : IXenialSourceGenerator
{
    private const string xenialTypeForwardedTypes = "XenialTypeForwardedTypes";
    public const string GenerateXenialTypeForwardedTypesMSBuildProperty = $"Generate{xenialTypeForwardedTypes}";

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types)
    {
        compilation = GenerateTypeForwardedTypes(context, compilation);
        return compilation;
    }

    private static Compilation GenerateTypeForwardedTypes(GeneratorExecutionContext context, Compilation compilation)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateXenialTypeForwardedTypesMSBuildProperty}", out var generateXenialTypeForwardedTypesAttrStr))
        {
            if (bool.TryParse(generateXenialTypeForwardedTypesAttrStr, out var xenialTypeForwardedTypesAttr))
            {
                if (!xenialTypeForwardedTypesAttr)
                {
                    return compilation;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            GenerateXenialTypeForwardedTypesMSBuildProperty,
                            generateXenialTypeForwardedTypesAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        foreach (var pair in Xenial.TypeForwardedTypes.TypeForwards)
        {
            var (source, syntaxTree) = GenerateTypeForwardedTypes(
                pair.Value,
                (CSharpParseOptions)context.ParseOptions,
                context.GetDefaultAttributeModifier(),
                context.CancellationToken
            );

            context.AddSource($"{pair.Key}.g.cs", source);
            compilation = compilation.AddSyntaxTrees(syntaxTree);
        }

        return compilation;
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateTypeForwardedTypes(
        string sourceEncoded,
        CSharpParseOptions? parseOptions = null,
        string visibility = "internal",
        CancellationToken cancellationToken = default)
    {
        parseOptions = parseOptions ?? CSharpParseOptions.Default;

        var syntaxWriter = CurlyIndenter.Create();

        var sourceDecoded = Base64Decode(sourceEncoded);
        sourceDecoded = sourceDecoded.Replace("{visibility}", visibility);

        syntaxWriter.Write(sourceDecoded);

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }

    private static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
