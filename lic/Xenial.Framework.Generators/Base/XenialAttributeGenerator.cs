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

namespace Xenial.Framework.Generators.Base;
public abstract record XenialAttributeGenerator(bool AddSource = true) : IXenialSourceGenerator
{
    protected const string XenialNamespace = "Xenial";

    public abstract string AttributeName { get; }

    public virtual string AttributeNamespace => XenialNamespace;

    public string AttributeFullName => $"{AttributeNamespace}.{AttributeName}";

    public string GenerateAttributeMSBuildProperty => $"Generate{AttributeName}";

    public virtual string AttributeVisibilityMSBuildProperty => AttributeModifiers.XenialAttributesVisibilityMSBuildProperty;

    public virtual bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, IList<string> addedSourceFiles)
        => GenerateAttribute(context, compilation ?? throw new ArgumentNullException(nameof(compilation)), addedSourceFiles ?? throw new ArgumentNullException(nameof(addedSourceFiles)));

    private Compilation GenerateAttribute(GeneratorExecutionContext context, Compilation compilation, IList<string> addedSourceFiles)
    {
        // Attribute is already declared in user code, skip generation
        var namedTypeSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
        if (namedTypeSymbol is not null)
        {
            return compilation;
        }

        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateAttributeMSBuildProperty}", out var generateXenialAttrStr))
        {
            if (bool.TryParse(generateXenialAttrStr, out var generateXenialAttr))
            {
                if (!generateXenialAttr)
                {
                    return compilation;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            GenerateAttributeMSBuildProperty,
                            generateXenialAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var (source, syntaxTree) = GenerateAttribute(
            (CSharpParseOptions)context.ParseOptions,
            context.GetDefaultAttributeModifier(),
            context.CancellationToken
        );

        if (AddSource)
        {
            var fileName = $"{AttributeName}.g.cs";
            addedSourceFiles.Add(fileName);
            context.AddSource(fileName, source);
        }

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    private (SourceText source, SyntaxTree syntaxTree) GenerateAttribute(
        CSharpParseOptions? parseOptions = null,
        string visibility = "internal",
        CancellationToken cancellationToken = default)
    {
        parseOptions ??= CSharpParseOptions.Default;

        var syntaxWriter = CurlyIndenter.Create();

        syntaxWriter = CreateAttribute(syntaxWriter, visibility);

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }

    protected abstract CurlyIndenter CreateAttribute(CurlyIndenter syntaxWriter, string visibility);
}
