﻿using System;
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

public record XenialDuckTypesGenerator(bool AddSource = true) : IXenialSourceGenerator
{
    private const string xenialDuckTypes = "XenialDuckTypes";
    public const string GenerateXenialDuckTypesMSBuildProperty = $"Generate{xenialDuckTypes}";

    public bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public Compilation Execute(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<TypeDeclarationSyntax> types,
        IList<string> addedSourceFiles
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = types ?? throw new ArgumentNullException(nameof(types));
        _ = addedSourceFiles ?? throw new ArgumentNullException(nameof(addedSourceFiles));

        compilation = GenerateDuckTypes(context, compilation, addedSourceFiles);
        return compilation;
    }

    private Compilation GenerateDuckTypes(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<string> addedSourceFiles
    )
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateXenialDuckTypesMSBuildProperty}", out var generateXenialTypeForwardedTypesAttrStr))
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
                            GenerateXenialDuckTypesMSBuildProperty,
                            generateXenialTypeForwardedTypesAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var isExternalInit = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.IsExternalInit");
        if (isExternalInit is null)
        {
            var (source, syntaxTree, name) = GenerateDuckTypes(
                (CSharpParseOptions)context.ParseOptions,
                context.CancellationToken
            );

            if (AddSource)
            {
                var hintName = $"{name}.g.cs";
                if (!addedSourceFiles.Contains(hintName))
                {
                    context.AddSource(hintName, source);
                }
            }

            compilation = compilation.AddSyntaxTrees(syntaxTree);

        }

        return compilation;
    }

    public static (SourceText source, SyntaxTree syntaxTree, string name) GenerateDuckTypes(
        CSharpParseOptions? parseOptions = null,
        CancellationToken cancellationToken = default)
    {
        parseOptions = parseOptions ?? CSharpParseOptions.Default;

        var builder = CurlyIndenter.Create();

        builder.WriteLine("// <auto-generated />");
        builder.WriteLine();
        builder.WriteLine("#if !NET5_0_OR_GREATER");
        builder.WriteLine("using System.ComponentModel;");

        using (builder.OpenBrace("namespace System.Runtime.CompilerServices"))
        {
            builder.WriteLine("[EditorBrowsable(EditorBrowsableState.Never)]");
            using (builder.OpenBrace("internal static class IsExternalInit")) { }
        }

        builder.WriteLine("#endif");

        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree, "_XenialDuckTypes");
    }
}
