
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using VerifyTests;

using VerifyXunit;

using Xunit;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ImageNamesGeneratorTests
{
#if FULL_FRAMEWORK || NETCOREAPP3_1
    static ImageNamesGeneratorTests() => RegisterModuleInitializers.RegisterVerifiers();
#endif

    private const string imageNamesBuildPropertyName = "build_property.GenerateXenialImageNamesAttribute";
    private const string compilationName = "AssemblyName";

    [Fact]
    public async Task EmitsImageNamesAttributeByDefault()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesNotEmitImageNamesAttributeIfOptedOut()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesEmitImageNamesAttributeIfOptedIn()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "true"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesEmitDiagnosticIfNotBoolean()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "ABC"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task EmitsCustomModifier()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(
                    new MockAnalyzerConfigOptions("build_property.XenialAttributesVisibility", "public")
                )
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesEmitDiagnosticIfNotPartial()
    {
        var compilation = CSharpCompilation.Create(compilationName).AddInlineXenialImageNamesAttribute();

        var syntax = @"using Xenial; namespace MyProject { [XenialImageNames(Foo = 123)] public class MyNonPartialClass{ } }";
        var syntaxTree =
            CSharpSyntaxTree.ParseText(
                syntax,
                new CSharpParseOptions(LanguageVersion.Default),
                path: "MyNonPartialClass.cs"
            );

        compilation = compilation.AddSyntaxTrees(syntaxTree);

        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesEmitDiagnosticIfInGlobalNamespace()
    {
        var compilation = CSharpCompilation.Create(compilationName).AddInlineXenialImageNamesAttribute();

        var syntax = @"using Xenial; [XenialImageNames(Foo = 123)] public partial class MyGlobalClass { }";
        var syntaxTree =
            CSharpSyntaxTree.ParseText(
                syntax,
                new CSharpParseOptions(LanguageVersion.Default),
                path: "MyNonPartialClass.cs"
            );

        compilation = compilation.AddSyntaxTrees(syntaxTree);

        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesNotEmitDiagnosticIfPartial()
    {
        var compilation = CSharpCompilation.Create(compilationName).AddInlineXenialImageNamesAttribute();

        var syntax = @"namespace MyProject { [Xenial.XenialImageNames] public partial class MyPartialClass { } }";

        compilation = compilation.AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                syntax,
                new CSharpParseOptions(LanguageVersion.Default),
                path: "MyPartialClass.cs"
            ));

        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesNotEmitDiagnosticIfAttributeIsNotApplied()
    {
        var compilation = CSharpCompilation.Create(compilationName).AddInlineXenialImageNamesAttribute();

        var syntax = @"namespace MyProject { [System.Obsolete]public class MyPartialClassWithoutAttribute{ } }";

        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, new CSharpParseOptions(LanguageVersion.Default)));

        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task BasicConstantGeneration()
    {
        var compilation = CSharpCompilation.Create(compilationName);

        var syntax = @"namespace MyProject { [Xenial.XenialImageNames] public partial class BasicImageNames { } }";

        compilation = compilation.AddInlineXenialImageNamesAttribute();

        compilation = compilation.AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                syntax,
                new CSharpParseOptions(LanguageVersion.Default),
                "BasicImageNames.cs"
            ));

        XenialImageNamesGenerator generator = new();

        var mockAdditionalText = new MockAdditionalText("Images/MyPicture.png");

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
                .WithAdditionalTreeOptions(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty.Add(mockAdditionalText, new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"))),
            additionalTexts: new[]
            {
                mockAdditionalText
            }
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    // BEWARE: 🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉🐉
    // For whatever weird reason, we need to generate the attribute beforehand in our tests.
    // otherwise the attributes get not "populated" correctly.
    // For this we use the generate `AddInlineXenialImageNamesAttribute`
    // method of the builder to avoid too much code duplication
    // It's for whatever reason important that the attribute is public in our tests
    // however in production it's works fine with internal visibility...
    [UsesVerify]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Test classes are fine for grouping")]
    public class AttributeDrivenTests
    {
        [Fact]
        public async Task SmartCommentsGeneration()
        {
            var syntax = @"namespace MyProject { [Xenial.XenialImageNames(SmartComments = true)] public partial class ImageNamesWithSmartComments{ } }";
            var syntaxTree = CSharpSyntaxTree.ParseText(
                   syntax,
                   new CSharpParseOptions(LanguageVersion.Default),
                   "ImageNamesWithSmartComments.cs"
            );

            var compilation = CSharpCompilation.Create(
                compilationName,
                syntaxTrees: new[] { syntaxTree },
                references: DefaultReferenceAssemblies,
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            ).AddInlineXenialImageNamesAttribute("public");

            XenialImageNamesGenerator generator = new();

            var mockAdditionalTexts = new[]
            {
                new MockAdditionalText("Images/MyImage.png"),
                new MockAdditionalText("Images/MyImage_32x32.png"),
                new MockAdditionalText("Images/MyImage_48x48.png"),
            };

            var additionalTreeOptions = ImmutableDictionary<object, AnalyzerConfigOptions>.Empty;

            foreach (var mockAdditionalText in mockAdditionalTexts)
            {
                additionalTreeOptions = additionalTreeOptions.Add(mockAdditionalText, new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"));
            }

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                new[] { generator },
                optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                    .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
                    .WithAdditionalTreeOptions(additionalTreeOptions),
                    additionalTexts: mockAdditionalTexts
            );

            (driver, var diagnostics, var ex, _) = driver.CompileAndLoadType(compilation, "MyProject.ImageNamesWithSizes");

            VerifyDiagnostics(diagnostics, ex);

            var settings = new VerifySettings();
            settings.UniqueForTargetFrameworkAndVersion();
            await Verifier.Verify(driver, settings);
        }

        [Fact]
        public async Task ResourceAccessorsGeneration()
        {
            var syntax = @"namespace MyProject { [Xenial.XenialImageNames(ResourceAccessors = true, SmartComments = true)] public partial class ImageNamesResourceAccessors { } }";
            var syntaxTree = CSharpSyntaxTree.ParseText(
                   syntax,
                   new CSharpParseOptions(LanguageVersion.Default),
                   "ResourceAccessors.cs"
            );

            var compilation = CSharpCompilation.Create(
                compilationName,
                syntaxTrees: new[] { syntaxTree },
                references: DefaultReferenceAssemblies,
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            ).AddInlineXenialImageNamesAttribute("public");

            XenialImageNamesGenerator generator = new();

            var mockAdditionalTexts = new[]
            {
                new MockAdditionalText("Images/MyImage.png"),
                new MockAdditionalText("Images/MyImage_32x32.png"),
                new MockAdditionalText("Images/MyImage_48x48.png"),
            };

            var additionalTreeOptions = ImmutableDictionary<object, AnalyzerConfigOptions>.Empty;

            foreach (var mockAdditionalText in mockAdditionalTexts)
            {
                additionalTreeOptions = additionalTreeOptions.Add(mockAdditionalText, new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"));
            }

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                new[] { generator },
                optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                    .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
                    .WithAdditionalTreeOptions(additionalTreeOptions),
                    additionalTexts: mockAdditionalTexts
            );

            (driver, var diagnostics, var ex, _) = driver.CompileAndLoadType(compilation, "MyProject.ResourceAccessors");

            VerifyDiagnostics(diagnostics, ex);

            var settings = new VerifySettings();
            settings.UniqueForTargetFrameworkAndVersion();
            await Verifier.Verify(driver, settings);
        }

        [Fact]
        public async Task SizesGeneration()
        {
            var syntax = @"namespace MyProject { [Xenial.XenialImageNames(Sizes = true)] public partial class ImageNamesWithSizes { } }";
            var syntaxTree = CSharpSyntaxTree.ParseText(
                   syntax,
                   new CSharpParseOptions(LanguageVersion.Default),
                   "ImageNamesWithSizes.cs"
            );

            var compilation = CSharpCompilation.Create(
                compilationName,
                syntaxTrees: new[] { syntaxTree },
                references: DefaultReferenceAssemblies,
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            ).AddInlineXenialImageNamesAttribute("public");

            XenialImageNamesGenerator generator = new();

            var mockAdditionalTexts = new[]
            {
                new MockAdditionalText("Images/MyImage.png"),
                new MockAdditionalText("Images/MyImage_32x32.png"),
                new MockAdditionalText("Images/MyImage_48x48.png"),
            };

            var additionalTreeOptions = ImmutableDictionary<object, AnalyzerConfigOptions>.Empty;

            foreach (var mockAdditionalText in mockAdditionalTexts)
            {
                additionalTreeOptions = additionalTreeOptions.Add(mockAdditionalText, new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"));
            }

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                new[] { generator },
                optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                    .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
                    .WithAdditionalTreeOptions(additionalTreeOptions),
                    additionalTexts: mockAdditionalTexts
            );

            (driver, var diagnostics, var ex, _) = driver.CompileAndLoadType(compilation, "MyProject.ImageNamesWithSizes");

            VerifyDiagnostics(diagnostics, ex);

            var settings = new VerifySettings();
            settings.UniqueForTargetFrameworkAndVersion();
            await Verifier.Verify(driver, settings);
        }


        [Fact]
        public async Task SubFolderBasic()
        {
            var syntax = @"namespace MyProject { [Xenial.XenialImageNames(SmartComments = true)] public partial class SubFolderImages { } }";
            var syntaxTree = CSharpSyntaxTree.ParseText(
                   syntax,
                   new CSharpParseOptions(LanguageVersion.Default),
                   "SubFolderImages.cs"
            );

            var compilation = CSharpCompilation.Create(
                compilationName,
                syntaxTrees: new[] { syntaxTree },
                references: DefaultReferenceAssemblies,
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            ).AddInlineXenialImageNamesAttribute("public");

            XenialImageNamesGenerator generator = new();

            var mockAdditionalTexts = new[]
            {
                new MockAdditionalText("Images/MyImage1.png"),
                new MockAdditionalText("Images/MySimpleFolder/MyImage2.png"),
                new MockAdditionalText("Images/MoreComplex/Folder/Inside/Folder/MyImage2.png"),
            };

            var additionalTreeOptions = ImmutableDictionary<object, AnalyzerConfigOptions>.Empty;

            foreach (var mockAdditionalText in mockAdditionalTexts)
            {
                additionalTreeOptions = additionalTreeOptions.Add(mockAdditionalText, new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"));
            }

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                new[] { generator },
                optionsProvider: MockAnalyzerConfigOptionsProvider.Empty
                    .WithGlobalOptions(new MockAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
                    .WithAdditionalTreeOptions(additionalTreeOptions),
                    additionalTexts: mockAdditionalTexts
            );

            (driver, var diagnostics, var ex, _) = driver.CompileAndLoadType(compilation, "MyProject.SubFolderImages");

            VerifyDiagnostics(diagnostics, ex);

            var settings = new VerifySettings();
            settings.UniqueForTargetFrameworkAndVersion();

            await Verifier.Verify(driver, settings);
        }

        private static void VerifyDiagnostics(ImmutableArray<Diagnostic> diagnostics, Exception? ex)
        {
            if (diagnostics.Length > 0 && ex is not null)
            {
                throw new AggregateException(new ArgumentException(string.Join(
                    Environment.NewLine,
                    diagnostics.Select(diag => new DiagnosticFormatter().Format(diag))
                )), ex);
            }
        }
    }
}

internal static class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialImageNamesAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialImageNamesGenerator.GenerateXenialImageNamesAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    public static (
        GeneratorDriver driver,
        ImmutableArray<Diagnostic> diagnostics,
        Exception? emitException,
        Type? generatedType
    ) CompileAndLoadType(
        this GeneratorDriver driver,
        Compilation compilation,
        string typeToLoad
    )
    {
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var compilation2, out var diag);

        using var stream = new MemoryStream();
        var emitResults = compilation2.Emit(stream);
        if (emitResults is not null)
        {
            stream.Position = 0;
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                var assembly = Assembly.Load(stream.ToArray());

                return (driver, emitResults.Diagnostics, null, assembly.GetType(typeToLoad));
            }
            catch (Exception ex)
            {
                return (driver, emitResults.Diagnostics, ex, null);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        return (driver, ImmutableArray<Diagnostic>.Empty, null, null);

    }
}
public class NotPartialImageNames
{
}

[XenialImageNames]
public partial class PartialImageNames { }
