using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Basic.Reference.Assemblies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using VerifyTests;

using VerifyXunit;

using Xunit;

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
                    new MockAnalyzerConfigOptions("build_property.XenialAttributesModifier", "public")
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

    [Fact]
    public async Task SizesGeneration()
    {
        var compilation = CSharpCompilation.Create(
            compilationName,
            references: ReferenceAssemblies.Net60,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var syntax = @"namespace MyProject { [Xenial.XenialImageNames(Sizes = true)] public partial class ImageNamesWithSizes { } }";

        compilation = compilation.AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                syntax,
                new CSharpParseOptions(LanguageVersion.Default),
                "ImageNamesWithSizes.cs"
            ));

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
                .WithAdditionalTreeOptions(additionalTreeOptions),
                additionalTexts: mockAdditionalTexts
        );

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var compilation2, out var diag);

        var comp = (CSharpCompilation)compilation2;
        var foo1 = comp.SyntaxTrees[0].ToString();
        var foo2 = comp.SyntaxTrees[1].ToString();

        using var stream = new MemoryStream();
        var emitResults = compilation2.Emit(stream);
        stream.Position = 0;
        var assembly = Assembly.Load(stream.ToArray());
        var method = assembly.GetType("MyProject.ImageNamesWithSizes");

        //var settings = new VerifySettings();
        //settings.UniqueForTargetFrameworkAndVersion();
        //await Verifier.Verify(driver, settings);
    }
}

internal static class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialImageNamesAttribute(this CSharpCompilation compilation)
        => compilation.AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                "namespace Xenial { internal class XenialImageNamesAttribute : System.Attribute { public XenialImageNamesAttribute() { } } } ",
                new CSharpParseOptions(LanguageVersion.Default)
                )
        );
}
public class NotPartialImageNames
{
}

[Xenial.XenialImageNames]
public partial class PartialImageNames { }
