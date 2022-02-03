﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.Generators.Internal;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators;

public record XenialImageNamesGenerator(bool AddSource = true) : XenialBaseGenerator(AddSource)
{
    private const string xenialImageNamesAttributeName = "XenialImageNamesAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialImageNamesAttributeFullName = $"{xenialNamespace}.{xenialImageNamesAttributeName}";
    public const string GenerateXenialImageNamesAttributeMSBuildProperty = $"Generate{xenialImageNamesAttributeName}";

    private const string markAsXenialImageSourceMetadataAttribute = "XenialImageNames";

    private const string imagesBaseFolder = "Images";

    public override bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public override Compilation Execute(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<TypeDeclarationSyntax> types,
        IList<string> addedSourceFiles
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = types ?? throw new ArgumentNullException(nameof(types));
        _ = addedSourceFiles ?? throw new ArgumentNullException(nameof(addedSourceFiles));

        context.CancellationToken.ThrowIfCancellationRequested();

        var globalOptions = new GlobalOptions(context);

        var generateXenialImageNamesAttribute = compilation.GetTypeByMetadataName(xenialImageNamesAttributeFullName);

        if (generateXenialImageNamesAttribute is null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return compilation;
        }

        foreach (var @class in types)
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
            builder.WriteLine("using System.Runtime.CompilerServices;");
            builder.WriteLine();

            if (IsInGlobalNamespace(context, compilation, classSymbol, xenialImageNamesAttributeName, @class.GetLocation(), out compilation))
            {
                return compilation;
            }

            using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
            {
                var features = new Features(@attribute);

                var defaultSize = attribute.GetAttributeValue(AttributeNames.DefaultImageSize, AttributeNames.DefaultImageSizeValue);
                if (!SanitizeSize(context, defaultSize))
                {
                    return compilation;
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
            }

            compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles);
        }

        return compilation;
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
        TypeDeclarationSyntax @class,
        CurlyIndenter builder,
        IList<string> addedSourceFiles
    )
    {
        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);

        var fileName = Path.GetFileNameWithoutExtension(@class.SyntaxTree.FilePath);

        if (fileName is "")
        {
            fileName = Guid.NewGuid().ToString();
        }

        var hintName = $"{fileName}.g.cs";
        if (!addedSourceFiles.Contains(hintName))
        {
            context.AddSource(hintName, source);
            return compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));
        }

        return compilation;
    }

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
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
                    GeneratorDiagnostics.ClassNeedsToBePartialWhenUsingAttribute(xenialImageNamesAttributeFullName),
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
        using (builder.OpenBrace($"partial {(Class.IsRecord ? "record" : "class")} {Class.Name}"))
        {
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
                    using (builder.OpenBrace($"{modifier} partial class Size{imageInfoGroup.suffix}"))
                    {
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
                    }
                }
            }
            else
            {
                foreach (var imageInfo in Images)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    GenerateImageNameConstant(Attribute, builder, modifier, imageInfo);
                }

                if (Features.ResourceAccessors)
                {
                    builder.WriteLine();
                    using (builder.OpenBrace($"{modifier} static class ResourceNames"))
                    {
                        foreach (var imageInfo in Images)
                        {
                            GenerateResourceNameConstant(Attribute, builder, modifier, imageInfo);
                        }
                    }

                    builder.WriteLine();

                    using (builder.OpenBrace($"{modifier} static class AsStream"))
                    {
                        foreach (var imageInfo in Images)
                        {
                            GenerateResourceStreamMethod(Class, Attribute, builder, modifier, imageInfo);
                        }
                    }

                    builder.WriteLine();

                    using (builder.OpenBrace($"{modifier} class AsBytes"))
                    {
                        foreach (var imageInfo in Images)
                        {
                            GenerateResourceBytesMethod(Class, Attribute, builder, modifier, imageInfo);
                        }
                    }

                    var semanticModel = context.Compilation.GetSemanticModel(context.Compilation.SyntaxTrees.First());

                    if (semanticModel is not null)
                    {
                        //We check if the image type is in reach of compilation
                        var imageType = context.Compilation.GetTypeByMetadataName("System.Drawing.Image");

                        if (imageType is not null)
                        {
                            using (builder.OpenBrace($"{modifier} class AsImage"))
                            {
                                foreach (var imageInfo in Images)
                                {
                                    GenerateResourceImageMethod(Class, Attribute, builder, modifier, imageInfo);
                                }
                            }
                        }
                    }
                }
            }
        }

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

        builder.WriteLine($"{modifier} static System.IO.Stream {(removeSuffix ? RemoveSuffix(imageInfo.Name, suffix) : imageInfo.Name)}()");
        using (builder.OpenBrace())
        {
            builder.WriteLine($"return typeof({@class.Name}).Assembly.GetManifestResourceStream(\"{imageInfo.ResourceName}\");");
        }
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
        using (builder.OpenBrace())
        {
            builder.WriteLine($"using(var stream = typeof({@class.Name}).Assembly.GetManifestResourceStream(\"{imageInfo.ResourceName}\"))");

            builder.WriteLine("using(var ms = new System.IO.MemoryStream())");
            using (builder.OpenBrace())
            {
                builder.WriteLine("stream.CopyTo(ms);");
                builder.WriteLine("return ms.ToArray();");
            }
        }
        builder.WriteLine();
    }

    private static void GenerateResourceImageMethod(
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

        using (builder.OpenBrace($"{modifier} static System.Drawing.Image {(removeSuffix ? RemoveSuffix(imageInfo.Name, suffix) : imageInfo.Name)}()"))
        using (builder.OpenBrace($"using(var stream = typeof({@class.Name}).Assembly.GetManifestResourceStream(\"{imageInfo.ResourceName}\"))"))
        {
            builder.WriteLine("return System.Drawing.Image.FromStream(stream);");
        }

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

