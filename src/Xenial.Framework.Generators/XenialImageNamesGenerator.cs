﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.MsBuild;

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

    private const string imagesBaseFolder = "Images";

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

        var globalOptions = new GlobalOptions(context);

        var compilation = GenerateAttribute(context);

        var generateXenialImageNamesAttribute = compilation.GetTypeByMetadataName(xenialImageNamesAttributeFullName);

        if (generateXenialImageNamesAttribute is null)
        {
            return;
        }

        foreach (var @class in syntaxReceiver.Classes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

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
            builder.WriteLine("using System.IO;");
            builder.WriteLine("using System.Runtime.CompilerServices;");
            builder.WriteLine();

            var isGlobalNamespace = classSymbol.ContainingNamespace.ToString() == "<global namespace>";
            if (isGlobalNamespace)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                        xenialImageNamesAttributeName
                    ), @class.GetLocation())
                );

                return;
            }

            builder.WriteLine($"namespace {@classSymbol.ContainingNamespace}");
            builder.OpenBrace();

            var features = new Features(@attribute);

            var defaultSize = attribute.GetAttributeValue(AttributeNames.DefaultImageSize, AttributeNames.DefaultImageSizeValue);
            if (!SanitizeSize(context, defaultSize))
            {
                return;
            }

            var images = GetImages(context, globalOptions).ToList();

            var imageClass = new ImagesClass(
                globalOptions,
                features,
                classSymbol,
                @attribute,
                images
            );

            builder = imageClass.ToString(context, builder);

            builder.CloseBrace();

            compilation = AddGeneratedCode(context, compilation, @class, builder);
        }
    }

    private static bool SanitizeSize(GeneratorExecutionContext context, string size)
    {
        //TODO: SanitizeSize eg. 16x16, 32x32 etc.
        if (string.IsNullOrEmpty(size))
        {
            return false;
        }

        return true;
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

        var isAttributeDeclared = symbol.IsAttributeDeclared(generateXenialImageNamesAttribute);

        if (isAttributeDeclared && !@class.HasModifier(SyntaxKind.PartialKeyword))
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

    private static AttributeData GetXenialImageNamesAttribute(INamedTypeSymbol symbol, INamedTypeSymbol generateXenialImageNamesAttribute)
        => symbol.GetAttribute(generateXenialImageNamesAttribute);

    private static IEnumerable<ImageInformation> GetImages(GeneratorExecutionContext context, GlobalOptions features)
    {
        var projectDirectory = features.GetProjectDirectory();
        var assemblyName = features.GetAssemblyName();

        foreach (var additionalText in context.AdditionalFiles)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var options = context.AnalyzerConfigOptions.GetOptions(additionalText);

            if (options is not null && options.TryGetValue($"build_metadata.AdditionalFiles.{markAsXenialImageSourceMetadataAttribute}", out var markedAsImageSourceStr))
            {
                if (bool.TryParse(markedAsImageSourceStr, out var markedAsImageSource))
                {
                    if (!markedAsImageSource)
                    {
                        continue;
                    }

                    var path = additionalText.Path;
                    var fileName = Path.GetFileName(path);
                    var name = Path.GetFileNameWithoutExtension(path);
                    var extension = Path.GetExtension(path);
                    var directory = Path.GetDirectoryName(path);

                    var imageRoot = Path.Combine(projectDirectory, imagesBaseFolder);

                    var relativePath = (path.StartsWith(imageRoot, StringComparison.InvariantCulture)
                        ? path.Substring(imageRoot.Length) : path).TrimStart('/', '\\');

                    yield return new ImageInformation(
                        path,
                        fileName,
                        name,
                        extension,
                        directory,
                        relativePath,
                        imagesBaseFolder,
                        projectDirectory,
                        assemblyName
                    );
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
        syntaxWriter.WriteLine($"using System.ComponentModel;");
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
        syntaxWriter.WriteLine($"public bool {AttributeNames.ResourceAccessors} {{ get; set; }}");

        syntaxWriter.WriteLine("[EditorBrowsable(EditorBrowsableState.Never)]");
        syntaxWriter.WriteLine($"public string {AttributeNames.DefaultImageSize} {{ get; set; }} = \"{AttributeNames.DefaultImageSizeValue}\";");


        syntaxWriter.CloseBrace();

        syntaxWriter.CloseBrace();

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
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

internal static class AttributeNames
{
    internal const string Sizes = nameof(Sizes);
    internal const string DefaultImageSize = nameof(DefaultImageSize);
    internal const string DefaultImageSizeValue = "16x16";

    internal const string SmartComments = nameof(SmartComments);
    internal const string ResourceAccessors = nameof(ResourceAccessors);

    internal const string SubClassFolders = nameof(SubClassFolders);
}

public record ImagesClass(
    GlobalOptions GlobalOptions,
    Features Features,
    INamedTypeSymbol Class,
    AttributeData Attribute,
    IEnumerable<ImageInformation> Images
)
{
    internal CurlyIndenter ToString(GeneratorExecutionContext context, CurlyIndenter builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.WriteLine("[CompilerGenerated]");
        //We don't need to specify any other modifier
        //because the user can decide if he want it to be an instance type.
        //We also don't need to specify the visibility for partial types
        builder.WriteLine($"partial class {Class.Name}");

        builder.OpenBrace();

        var modifier = Class.GetResultantVisibility() == SymbolVisibility.Public
            ? "public"
            : "internal";

        if (Features.Sizes)
        {
            var imagesWithoutSuffix = Images.Where(i => !i.IsSuffixed(GlobalOptions.DefaultImageSuffixes));
            var imagesWithSuffix = GlobalOptions.DefaultImageSuffixes.Select(suffix => new
            {
                suffix,
                images = Images.Where(i => i.IsSuffixed(suffix))
            }).Where(i => i.images.Any());

            foreach (var imageInfo in imagesWithoutSuffix)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                GenerateImageNameConstant(Attribute, builder, modifier, imageInfo);
            }

            foreach (var imageInfoGroup in imagesWithSuffix)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                builder.WriteLine();
                builder.WriteLine($"{modifier} partial class Size{imageInfoGroup.suffix}");
                builder.OpenBrace();

                foreach (var imageInfo in imageInfoGroup.images)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    GenerateImageNameConstant(
                        Attribute,
                        builder,
                        modifier,
                        imageInfo,
                        removeSuffix: true,
                        suffix: imageInfoGroup.suffix
                    );
                }

                builder.CloseBrace();
            }
        }
        else
        {
            foreach (var imageInfo in Images)
            {
                Console.WriteLine(imageInfo);

                context.CancellationToken.ThrowIfCancellationRequested();
                GenerateImageNameConstant(Attribute, builder, modifier, imageInfo);
            }

            if (Features.ResourceAccessors)
            {
                builder.WriteLine();
                builder.WriteLine($"{modifier} static class ResourceNames");
                builder.OpenBrace();
                foreach (var imageInfo in Images)
                {
                    GenerateResourceNameConstant(Attribute, builder, modifier, imageInfo);
                }

                builder.CloseBrace();

                builder.WriteLine();

                builder.WriteLine($"{modifier} static class AsStream");
                builder.OpenBrace();

                foreach (var imageInfo in Images)
                {
                    GenerateResourceStreamMethod(Class, Attribute, builder, modifier, imageInfo);
                }

                builder.CloseBrace();

                builder.WriteLine();

                builder.WriteLine($"{modifier} class AsBytes");
                builder.OpenBrace();

                foreach (var imageInfo in Images)
                {
                    GenerateResourceBytesMethod(Class, Attribute, builder, modifier, imageInfo);
                }

                builder.CloseBrace();


            }
        }

        builder.CloseBrace();

        return builder;
    }

    private static void GenerateImageNameConstant(
        AttributeData attribute,
        CurlyIndenter builder,
        string modifier,
        ImageInformation imageInfo,
        bool removeSuffix = true,
        string suffix = ""
    )
    {
        if (@attribute.IsAttributeSet(AttributeNames.SmartComments))
        {
            builder.WriteLine($"//![]({imageInfo.Path})");
        }

        static string RemoveSuffix(string imageName, string suffix)
        {
            if (imageName.EndsWith(suffix, StringComparison.InvariantCulture))
            {
                return imageName.Substring(0, imageName.Length - suffix.Length).TrimEnd('_');
            }
            return imageName;
        }

        builder.WriteLine($"{modifier} const string {(removeSuffix ? RemoveSuffix(imageInfo.Name, suffix) : imageInfo.Name)} = \"{imageInfo.Name}\";");
    }

    private static void GenerateResourceNameConstant(
        AttributeData attribute,
        CurlyIndenter builder,
        string modifier,
        ImageInformation imageInfo,
        bool removeSuffix = true,
        string suffix = ""
    )
    {
        if (@attribute.IsAttributeSet(AttributeNames.SmartComments))
        {
            builder.WriteLine($"//![]({imageInfo.Path})");
        }

        static string RemoveSuffix(string imageName, string suffix)
        {
            if (imageName.EndsWith(suffix, StringComparison.InvariantCulture))
            {
                return imageName.Substring(0, imageName.Length - suffix.Length).TrimEnd('_');
            }
            return imageName;
        }

        builder.WriteLine($"{modifier} const string {(removeSuffix ? RemoveSuffix(imageInfo.Name, suffix) : imageInfo.Name)} = \"{imageInfo.ResourceName}\";");
    }


    private static void GenerateResourceStreamMethod(
        INamedTypeSymbol @class,
        AttributeData attribute,
        CurlyIndenter builder,
        string modifier,
        ImageInformation imageInfo,
        bool removeSuffix = true,
        string suffix = ""
    )
    {
        if (@attribute.IsAttributeSet(AttributeNames.SmartComments))
        {
            builder.WriteLine($"//![]({imageInfo.Path})");
        }

        static string RemoveSuffix(string imageName, string suffix)
        {
            if (imageName.EndsWith(suffix, StringComparison.InvariantCulture))
            {
                return imageName.Substring(0, imageName.Length - suffix.Length).TrimEnd('_');
            }
            return imageName;
        }

        builder.WriteLine($"{modifier} static Stream {(removeSuffix ? RemoveSuffix(imageInfo.Name, suffix) : imageInfo.Name)}()");
        builder.OpenBrace();
        builder.WriteLine($"return typeof({@class.Name}).Assembly.GetManifestResourceStream(\"{imageInfo.ResourceName}\");");
        builder.CloseBrace();
        builder.WriteLine();
    }


    private static void GenerateResourceBytesMethod(
        INamedTypeSymbol @class,
        AttributeData attribute,
        CurlyIndenter builder,
        string modifier,
        ImageInformation imageInfo,
        bool removeSuffix = true,
        string suffix = ""
    )
    {
        if (@attribute.IsAttributeSet(AttributeNames.SmartComments))
        {
            builder.WriteLine($"//![]({imageInfo.Path})");
        }

        static string RemoveSuffix(string imageName, string suffix)
        {
            if (imageName.EndsWith(suffix, StringComparison.InvariantCulture))
            {
                return imageName.Substring(0, imageName.Length - suffix.Length).TrimEnd('_');
            }
            return imageName;
        }

        builder.WriteLine($"{modifier} static byte[] {(removeSuffix ? RemoveSuffix(imageInfo.Name, suffix) : imageInfo.Name)}()");
        builder.OpenBrace();
        builder.WriteLine($"using(var stream = typeof({@class.Name}).Assembly.GetManifestResourceStream(\"{imageInfo.ResourceName}\"))");

        builder.WriteLine("using(var ms = new MemoryStream())");
        builder.OpenBrace();
        builder.WriteLine("stream.CopyTo(ms);");
        builder.WriteLine("return ms.ToArray();");
        builder.CloseBrace();
        builder.CloseBrace();
        builder.WriteLine();
    }
}

public record Features(AttributeData Attribute)
{
    public bool Sizes => Attribute.IsAttributeSet(AttributeNames.Sizes);
    public bool ResourceAccessors => Attribute.IsAttributeSet(AttributeNames.ResourceAccessors);
}

public record GlobalOptions(GeneratorExecutionContext Context)
{
    public IList<string> DefaultImageSuffixes { get; } = new[]
    {
        "12x12",
        "16x16",
        "24x24",
        "32x32",
        "48x48",
    };

    public string GetProjectDirectory()
    {
        if (Context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.ProjectDir", out var projectDir))
        {
            return projectDir;
        }

        return string.Empty;
    }

    public string GetAssemblyName()
    {
        if (Context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.AssemblyName", out var assemblyName))
        {
            return assemblyName;
        }

        return string.Empty;
    }
}

public record struct ImageInformation(
    string Path,
    string FileName,
    string Name,
    string Extension,
    string Directory,
    string RelativePath,
    string BaseDirectory,
    string ProjectDirectory,
    string AssemblyName)
{
    public bool IsSuffixed(string suffix)
        => Name.EndsWith(suffix, StringComparison.InvariantCulture);

    public bool IsSuffixed(IEnumerable<string> suffixes)
        => suffixes.Any(IsSuffixed);

    public string RelativeDirectory
        => System.IO.Path.GetDirectoryName(RelativePath);

    public string ResourceName =>
        string.Join(".", new[]
        {
            AssemblyName,
            BaseDirectory
        }.Concat(
            RelativePath.Replace('/', '.')
                .Replace('\\', '.')
                .Split('.')
            ).Where(
                s => !string.IsNullOrEmpty(s)
            )
        );
}

internal enum SymbolVisibility
{
    Public,
    Internal,
    Private,
}
