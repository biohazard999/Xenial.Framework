﻿

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Xenial.Framework.MsBuild;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Xenial.Framework.Generators;

[Generator]
public class XenialImageNamesGenerator : ISourceGenerator
{
    private const string xenialDebugSourceGenerators = "XenialDebugSourceGenerators";

    private const string xenialImageNamesAttributeName = "XenialImageNamesAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialImageNamesAttributeFullName = $"{xenialNamespace}.{xenialImageNamesAttributeName}";
    private const string generateXenialImageNamesAttributeMSBuildProperty = $"Generate{xenialImageNamesAttributeName}";

    private const string markAsXenialImageSourceMetadataAttribute = "XenialImageNames";

    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    /// <summary>
    /// Receives all the classes that have the Xenial.XenialImageNamesAttribute set.
    /// </summary>
    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<ClassDeclarationSyntax> Classes { get; } = new();

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
                    Classes.Add(classDeclarationSyntax);
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

        context.CancellationToken.ThrowIfCancellationRequested();

        CheckForDebugger(context);

        var compilation = GenerateAttribute(context);

        var generateXenialImageNamesAttribute = compilation.GetTypeByMetadataName(xenialImageNamesAttributeFullName);

        if (generateXenialImageNamesAttribute is null)
        {
            return;
        }

        foreach (var @class in syntaxReceiver.Classes)
        {
            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, generateXenialImageNamesAttribute);
            if (!isAttributeDeclared || semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            var @attribute = GetXenialImageNamesAttribute(@classSymbol, generateXenialImageNamesAttribute);

            var builder = CurlyIndenter.Create();

            builder.WriteLine("// <auto-generated />");
            builder.WriteLine();
            builder.WriteLine("using System;");
            builder.WriteLine("using System.Runtime.CompilerServices;");
            builder.WriteLine();

            builder.WriteLine($"namespace {@classSymbol.ContainingNamespace}");
            builder.OpenBrace();

            builder.WriteLine("[CompilerGenerated]");
            //We don't need to specify any other modifier
            //because the user can decide if he want it to be instanciatable.
            //We also don't need to specify the visibility for partial types
            builder.WriteLine($"partial class {classSymbol.Name}");

            builder.OpenBrace();

            var modifier = GetResultantVisibility(classSymbol) == SymbolVisibility.Public
                ? "public"
                : "internal";

            foreach (var imageInfo in GetImages(context))
            {
                GenerateImageNameConstant(attribute, builder, modifier, imageInfo);
            }

            builder.CloseBrace();

            builder.CloseBrace();

            compilation = AddGeneratedCode(context, compilation, @class, builder);
        }
    }

    private static void GenerateImageNameConstant(
        AttributeData attribute,
        CurlyIndenter builder,
        string modifier,
        ImageInformation imageInfo
    )
    {
        if (@attribute.IsAttributeSet(AttributeNames.SmartComments))
        {
            builder.WriteLine($"//![]({imageInfo.Path})");
        }

        builder.WriteLine($"{modifier} const string {imageInfo.Name} = \"{imageInfo.Name}\";");
    }

    private static Compilation AddGeneratedCode(
        GeneratorExecutionContext context,
        Compilation compilation,
        ClassDeclarationSyntax @class,
        CurlyIndenter builder
    )
    {
        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);

        var fileName = Path.GetFileNameWithoutExtension(@class.SyntaxTree.FilePath);

        if (fileName is "")
        {
            fileName = Guid.NewGuid().ToString();
        }

        context.AddSource($"{fileName}.g.cs", source);

        return compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));
    }

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        ClassDeclarationSyntax @class,
        INamedTypeSymbol generateXenialImageNamesAttribute
    )
    {
        var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);
        if (semanticModel is null)
        {
            return (semanticModel, null, false);
        }

        var symbol = semanticModel.GetDeclaredSymbol(@class, context.CancellationToken);

        if (symbol is null)
        {
            return (semanticModel, null, false);
        }

        var isAttributeDeclared = symbol
            .GetAttributes()
            .Any(m =>
                m.AttributeClass is not null
                //We can safely compare with ToString because it represents just the NamedTypeSymbol, not the attributes or overloads
                && m.AttributeClass.ToString() == generateXenialImageNamesAttribute.ToString()
             );

        if (isAttributeDeclared && !@class.Modifiers.Any(mod => mod.Text == Token(SyntaxKind.PartialKeyword).Text))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ClassNeedsToBePartial(xenialImageNamesAttributeFullName),
                    @class.GetLocation()
            ));

            return (semanticModel, symbol, false);
        }

        return (semanticModel, symbol, isAttributeDeclared);
    }

    private static AttributeData GetXenialImageNamesAttribute(
        INamedTypeSymbol symbol,
        INamedTypeSymbol generateXenialImageNamesAttribute
        ) => symbol.GetAttributes().First(m =>
              m.AttributeClass is not null
              //We can safely compare with ToString because it represents just the NamedTypeSymbol, not the attributes or overloads
              && m.AttributeClass.ToString() == generateXenialImageNamesAttribute.ToString()
             );

    private static IEnumerable<ImageInformation> GetImages(GeneratorExecutionContext context)
    {
        foreach (var additionalText in context.AdditionalFiles)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var options = context.AnalyzerConfigOptions.GetOptions(additionalText);

            if (options is not null && options.TryGetValue($"build_metadata.AdditionalFiles.{markAsXenialImageSourceMetadataAttribute}", out var markedAsImageSourceStr))
            {
                if (bool.TryParse(markedAsImageSourceStr, out var markedAsImageSource))
                {
                    if (markedAsImageSource)
                    {
                        var path = additionalText.Path;
                        var fileName = Path.GetFileName(path);
                        var name = Path.GetFileNameWithoutExtension(path);
                        var extension = Path.GetExtension(path);

                        yield return new ImageInformation(path, fileName, name, extension);
                    }
                }
                else
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            markAsXenialImageSourceMetadataAttribute,
                            markedAsImageSourceStr
                        ), null)
                    );
                }
            }
        }
    }

    private static Compilation GenerateAttribute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{generateXenialImageNamesAttributeMSBuildProperty}", out var generateXenialImageNamesAttrStr))
        {
            if (bool.TryParse(generateXenialImageNamesAttrStr, out var generateXenialImageNamesAttr))
            {
                if (!generateXenialImageNamesAttr)
                {
                    return compilation;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            generateXenialImageNamesAttributeMSBuildProperty,
                            generateXenialImageNamesAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var (source, syntaxTree) = GenerateXenialImageNamesAttribute(
            (CSharpParseOptions)context.ParseOptions,
            context.GetDefaultAttributeModifier(),
            context.CancellationToken
        );

        context.AddSource($"{xenialImageNamesAttributeName}.g.cs", source);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialImageNamesAttribute(
        CSharpParseOptions? parseOptions = null,
        string visibility = "internal",
        CancellationToken cancellationToken = default)
    {
        parseOptions = parseOptions ?? CSharpParseOptions.Default;

        var syntaxWriter = CurlyIndenter.Create();

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine();

        syntaxWriter.WriteLine($"namespace {xenialNamespace}");
        syntaxWriter.OpenBrace();

        syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]");
        syntaxWriter.WriteLine($"{visibility} sealed class {xenialImageNamesAttributeName} : Attribute");
        syntaxWriter.OpenBrace();
        syntaxWriter.WriteLine($"{visibility} {xenialImageNamesAttributeName}() {{ }}");

        syntaxWriter.WriteLine();

        //Properties need to be public in order to be used
        syntaxWriter.WriteLine($"public bool {AttributeNames.Sizes} {{ get; set; }}");
        syntaxWriter.WriteLine($"public bool {AttributeNames.SmartComments} {{ get; set; }}");
        syntaxWriter.WriteLine($"public string {AttributeNames.DefaultImageSize} {{ get; set; }} = \"{AttributeNames.DefaultImageSizeValue}\";");


        syntaxWriter.CloseBrace();

        syntaxWriter.CloseBrace();

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }

    internal static class AttributeNames
    {
        internal const string Sizes = nameof(Sizes);
        internal const string DefaultImageSize = nameof(DefaultImageSize);
        internal const string DefaultImageSizeValue = "16x16";

        internal const string SmartComments = nameof(SmartComments);
    }

    internal static SymbolVisibility GetResultantVisibility(ISymbol symbol)
    {
        // Start by assuming it's visible.
        var visibility = SymbolVisibility.Public;

        switch (symbol.Kind)
        {
            case SymbolKind.Alias:
                // Aliases are uber private.  They're only visible in the same file that they
                // were declared in.
                return SymbolVisibility.Private;

            case SymbolKind.Parameter:
                // Parameters are only as visible as their containing symbol
                return GetResultantVisibility(symbol.ContainingSymbol);

            case SymbolKind.TypeParameter:
                // Type Parameters are private.
                return SymbolVisibility.Private;
        }

        while (symbol != null && symbol.Kind != SymbolKind.Namespace)
        {
            switch (symbol.DeclaredAccessibility)
            {
                // If we see anything private, then the symbol is private.
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return SymbolVisibility.Private;

                // If we see anything internal, then knock it down from public to
                // internal.
                case Accessibility.Internal:
                case Accessibility.ProtectedAndInternal:
                    visibility = SymbolVisibility.Internal;
                    break;

                    // For anything else (Public, Protected, ProtectedOrInternal), the
                    // symbol stays at the level we've gotten so far.
            }

            symbol = symbol.ContainingSymbol;
        }

        return visibility;
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

internal static class AttributeDataExt
{
    internal static bool IsAttributeSet(this AttributeData attribute, string attributeName)
    {
        var namedArgument = attribute.NamedArguments.FirstOrDefault(argument => argument.Key == attributeName);

        if (namedArgument.Key == attributeName
            && namedArgument.Value.Kind is TypedConstantKind.Primitive
            && namedArgument.Value.Value is bool value)
        {
            return value;
        }

        return false;
    }
}

public record ImageInformation(string Path, string FileName, string Name, string Extension) { }

internal enum SymbolVisibility
{
    Public,
    Internal,
    Private,
}
