
using System;
using System.Collections.Generic;
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

public abstract class BaseGeneratorTests<TGenerator>
    where TGenerator : class, ISourceGenerator, new()
{
    protected const string CompilationName = "AssemblyName";
    protected const string XenialAttributesVisibility = "XenialAttributesVisibility";

#if FULL_FRAMEWORK || NETCOREAPP3_1
    static BaseGeneratorTests() => RegisterModuleInitializers.RegisterVerifiers();
#endif

    protected virtual TGenerator CreateGenerator()
        => new TGenerator();

    protected string BuildProperty(string property)
        => $"build_property.{property}";

    protected abstract string GeneratorEmitProperty { get; }

    protected async Task RunTest(
        Func<MockAnalyzerConfigOptionsProvider, MockAnalyzerConfigOptionsProvider>? analyzerOptions = null,
        Action<VerifySettings>? verifySettings = null,
        Func<CSharpCompilation, CSharpCompilation>? compilationOptions = null,
        Func<SyntaxTree[]>? syntaxTrees = null,
        Func<IEnumerable<AdditionalText>>? additionalTexts = null,
        string? typeToLoad = null
    )
    {
        var compilation = CSharpCompilation.Create(CompilationName,
                references: DefaultReferenceAssemblies,
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        if (compilationOptions is not null)
        {
            compilation = compilationOptions(compilation);
        }

        if (syntaxTrees is not null)
        {
            compilation = compilation.AddSyntaxTrees(syntaxTrees());
        }

        var generator = CreateGenerator();

        var mockOptions = MockAnalyzerConfigOptionsProvider.Empty;

        if (analyzerOptions is not null)
        {
            mockOptions = analyzerOptions(mockOptions);
        }

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: mockOptions,
            additionalTexts: additionalTexts == null ? null : additionalTexts()
        );

        var (driverAfterCompile, diagnostics, ex, _) = typeToLoad == null
            ? (driver.RunGenerators(compilation), ImmutableArray<Diagnostic>.Empty, (Exception?)null, (Type?)null)
            : driver.CompileAndLoadType(compilation, typeToLoad);

        VerifyDiagnostics(diagnostics, ex);

        driver = driverAfterCompile;

        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        verifySettings?.Invoke(settings);
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

    [Fact]
    public Task EmitsGenerateAttributeByDefault()
        => RunTest();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DoesNotEmitIfAttributeIfOpted(bool emitProperty)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), emitProperty.ToString())),
            settings => settings.UseParameters(emitProperty)
        );

    [Fact]
    public Task DoesCreateDiagnosticIfEmitAttributeMSBuildVariableIsNotABool()
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "ABC"))
        );

    [Fact]
    public Task DoesEmitCustomAttributeModifier()
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(XenialAttributesVisibility), "public"))
        );

    protected SyntaxTree BuildSyntaxTree(string fileName, string sourceText)
    {
        var syntaxTree =
            CSharpSyntaxTree.ParseText(
                sourceText,
                new CSharpParseOptions(LanguageVersion.Default),
                path: fileName
            );

        return syntaxTree;
    }
}

[UsesVerify]
public class ImageNamesGeneratorTests : BaseGeneratorTests<XenialImageNamesGenerator>
{
    protected override string GeneratorEmitProperty => "GenerateXenialImageNamesAttribute";

    private const string imageNamesBuildPropertyName = "build_property.GenerateXenialImageNamesAttribute";

    protected Task RunSourceTest(string fileName, string source)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialImageNamesAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            });

    protected Task RunSourceTestWithAdditionalFiles(string fileName, string source, string[] additionalFiles, string? typeToLoad = null)
        => RunSourceTestWithAdditionalFiles(fileName, source, additionalFiles.Select(f => new MockAdditionalText(f)), typeToLoad);

    protected Task RunSourceTestWithAdditionalFiles(string fileName, string source, IEnumerable<MockAdditionalText> additionalFiles, string? typeToLoad = null)
        => RunTest(
            options => options
                .WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false"))
                .WithAdditionalTreeOptions(
                    additionalFiles.ToImmutableDictionary(k => (object)k, _ => (AnalyzerConfigOptions)new MockAnalyzerConfigOptions("build_metadata.AdditionalFiles.XenialImageNames", "true"))
                ),
            compilationOptions: compilation => compilation.AddInlineXenialImageNamesAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            },
            additionalTexts: () => additionalFiles,
            typeToLoad: typeToLoad);

    [Fact]
    public Task DoesEmitDiagnosticIfNotPartial()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
namespace MyProject
{
    [XenialImageNames(Foo = 123)]
    public class MyNonPartialClass{ }
}");

    [Fact]
    public Task DoesEmitDiagnosticIfInGlobalNamespace()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
[XenialImageNames(Foo = 123)]
public partial class MyGlobalClass
{
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfPartial()
        => RunSourceTest("MyPartialClass.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfAttributeIsNotApplied()
        => RunSourceTest(string.Empty,
@"namespace MyProject
{
    [System.Obsolete]
    public class MyPartialClassWithoutAttribute{ }
}");

    [Fact]
    public Task BasicConstantGeneration()
        => RunSourceTestWithAdditionalFiles("BasicImageNames.cs",
            @"namespace MyProject { [Xenial.XenialImageNames] public partial class BasicImageNames { } }",
            new[]
            {
                "Images/MyPicture.png"
            });

    [UsesVerify]
    public class AttributeDrivenTests
    {
        private static Task RunSourceTestWithAdditionalFiles(string fileName, string source, string[] additionalFiles, string? typeToLoad = null)
            => new ImageNamesGeneratorTests()
                .RunSourceTestWithAdditionalFiles(fileName, source, additionalFiles, typeToLoad);

        [Fact]
        public Task SmartCommentsGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ImageNamesWithSmartComments.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames(SmartComments = true)]
    public partial class ImageNamesWithSmartComments{ }
}",
                new[]
                {
                    "Images/MyImage.png",
                    "Images/MyImage_32x32.png",
                    "Images/MyImage_48x48.png"
                },
                "MyProject.ImageNamesWithSizes"
            );

        [Fact]
        public Task ResourceAccessorsGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ResourceAccessors.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames(ResourceAccessors = true, SmartComments = true)]
    public partial class ImageNamesResourceAccessors { }
}",
                new[]
                {
                    "Images/MyImage.png",
                    "Images/MyImage_32x32.png",
                    "Images/MyImage_48x48.png"
                },
                "MyProject.ImageNamesResourceAccessors"
            );

        [Fact]
        public Task SizesGeneration()
            => RunSourceTestWithAdditionalFiles(
                "ImageNamesWithSizes.cs",
@"namespace MyProject
{
    [Xenial.XenialImageNames(Sizes = true)]
    public partial class ImageNamesWithSizes { }
}",
                new[]
                {
                    "Images/MyImage.png",
                    "Images/MyImage_32x32.png",
                    "Images/MyImage_48x48.png"
                },
                "MyProject.ImageNamesWithSizes"
            );

        [Fact(Skip = "Currently not Supported")]
        public async Task SubFolderBasic()
        {
            var syntax = @"namespace MyProject { [Xenial.XenialImageNames(SmartComments = true)] public partial class SubFolderImages { } }";
            var syntaxTree = CSharpSyntaxTree.ParseText(
                   syntax,
                   new CSharpParseOptions(LanguageVersion.Default),
                   "SubFolderImages.cs"
            );

            var compilation = CSharpCompilation.Create(
                CompilationName,
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
