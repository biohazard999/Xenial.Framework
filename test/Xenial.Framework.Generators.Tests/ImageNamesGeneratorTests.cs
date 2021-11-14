﻿using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

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
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
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
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(imageNamesBuildPropertyName, "true"))
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
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(imageNamesBuildPropertyName, "ABC"))
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

    [Fact]
    public async Task DoesEmitDiagnosticIfNotPartial()
    {
        var compilation = CSharpCompilation.Create(compilationName);

        var syntax = @"namespace MyProject { [Xenial.XenialImageNames] public class MyNonPartialClass{ } }";

        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, new CSharpParseOptions(LanguageVersion.Default)));

        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public async Task DoesNotEmitDiagnosticIfPartial()
    {
        var compilation = CSharpCompilation.Create(compilationName);

        var syntax = @"namespace MyProject { [Xenial.XenialImageNames] public partial class MyPartialClass{ } }";

        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, new CSharpParseOptions(LanguageVersion.Default)));

        XenialImageNamesGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: CompilerAnalyzerConfigOptionsProvider.Empty
                .WithGlobalOptions(new CompilerAnalyzerConfigOptions(imageNamesBuildPropertyName, "false"))
        );

        driver = driver.RunGenerators(compilation);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verifier.Verify(driver, settings);
    }
}

public class NotPartialImageNames
{
}

[Xenial.XenialImageNames]
public partial class PartialImageNames { }
