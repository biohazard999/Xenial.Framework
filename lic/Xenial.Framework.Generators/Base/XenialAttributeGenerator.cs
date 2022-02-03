
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Base;
public abstract record XenialAttributeGenerator(bool AddSource = true) : XenialBaseGenerator(AddSource)
{
    public abstract string AttributeName { get; }

    public string AttributeFullName => $"{AttributeNamespace}.{AttributeName}";

    public string GenerateAttributeMSBuildProperty => $"Generate{AttributeName}";

    public virtual string AttributeVisibilityMSBuildProperty => AttributeModifiers.XenialAttributesVisibilityMSBuildProperty;

    public override bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public override Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, IList<string> addedSourceFiles)
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

        var source = CreateAttribute(CurlyIndenter.Create(), context.GetDefaultAttributeModifier());

        return AddCode(context, compilation, addedSourceFiles, AttributeName, source.ToString());
    }

    protected abstract CurlyIndenter CreateAttribute(CurlyIndenter syntaxWriter, string visibility);
}
