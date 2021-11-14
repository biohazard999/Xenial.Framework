using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using VerifyTests;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class XpoBuilderGeneratorTests
{
#if FULL_FRAMEWORK || NETCOREAPP3_1
    static XpoBuilderGeneratorTests() => RegisterModuleInitializers.RegisterVerifiers();
#endif

    private const string xpoBuilderBuildPropertyName = "build_property.GenerateXenialXpoBuilderAttribute";
    private const string compilationName = "AssemblyName";

    [Fact]
    public async Task EmitsXpoBuilderAttributeByDefault()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialXpoBuilderGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesNotEmitXpoBuilderAttributeIfOptedOut()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialXpoBuilderGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(xpoBuilderBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesEmitXpoBuilderAttributeIfOptedIn()
    {
        var compilation = CSharpCompilation.Create(compilationName);
        XenialXpoBuilderGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(xpoBuilderBuildPropertyName, "true"))
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
        XenialXpoBuilderGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(xpoBuilderBuildPropertyName, "ABC"))
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
        XenialXpoBuilderGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(
                    new CompilerAnalyzerConfigOptions("build_property.XenialAttributesModifier", "public")
                )
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }
}

