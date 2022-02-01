using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using VerifyTests;

using VerifyXunit;

using Xunit;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public abstract class AttributeGeneratorBaseTests<TGenerator>
    where TGenerator : XenialAttributeGenerator
{
    protected const string CompilationName = "AssemblyName";

#if FULL_FRAMEWORK || NETCOREAPP3_1
    static BaseGeneratorTests() => RegisterModuleInitializers.RegisterVerifiers();
#endif
    protected abstract TGenerator CreateTargetGenerator();

    protected TGenerator CreateGeneratorWithoutAddSources()
        => CreateTargetGenerator() with { AddSources = false };

    protected TGenerator CreateGeneratorWithAddSources()
        => CreateTargetGenerator() with { AddSources = true };

    protected virtual XenialGenerator CreateGenerator(bool withSources = false)
    {
        var generator = new XenialGenerator();
        generator.Generators.Clear();
        return generator;
    }

    protected virtual IEnumerable<PortableExecutableReference> AdditionalReferences => Enumerable.Empty<PortableExecutableReference>();

    protected static CSharpCompilation CreateCompilation()
        => CSharpCompilation.Create(CompilationName,
                references: DefaultReferenceAssemblies,
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

    protected async Task RunTest(
        Func<MockAnalyzerConfigOptionsProvider, TGenerator, MockAnalyzerConfigOptionsProvider>? analyzerOptions = null,
        Action<VerifySettings>? verifySettings = null,
        bool withSources = false
    )
    {
        var compilation = CreateCompilation();

        var generator = CreateGenerator();

        var targetGenerator = withSources ? CreateGeneratorWithAddSources() : CreateGeneratorWithoutAddSources();

        generator.Generators.Add(
            targetGenerator
        );

        var mockOptions = MockAnalyzerConfigOptionsProvider.Empty;

        if (analyzerOptions is not null)
        {
            mockOptions = analyzerOptions(mockOptions, targetGenerator);
        }

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { generator },
            optionsProvider: mockOptions
        );

        driver = driver.RunGenerators(compilation);

        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        verifySettings?.Invoke(settings);
        await Verifier.Verify(driver, settings);
    }

    [Fact]
    public Task EmitsGenerateAttributeByDefault()
        => RunTest();

    protected string BuildProperty(string property)
        => $"build_property.{property}";

    [Fact]
    public Task EmitAttribute()
        => RunTest(withSources: true);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DoesEmitIfGenerateXenialAttributesMsBuildProperty(bool emitProperty)
        => RunTest(
            (options, gen) => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(gen.GenerateAttributeMSBuildProperty), emitProperty.ToString())),
            settings => settings.UseParameters(emitProperty),
            withSources: true
        );

    [Fact]
    public Task DoesCreateDiagnosticIfGenerateXenialAttributesMsBuildPropertyIsNotABool()
        => RunTest(
            (options, gen) => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(gen.GenerateAttributeMSBuildProperty), "ABC")),
            withSources: true
        );

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DoesEmitIfSpecificMsBuildProperty(bool emitProperty)
        => RunTest(
            (options, gen) => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(gen.GenerateAttributeMSBuildProperty), emitProperty.ToString())),
            settings => settings.UseParameters(emitProperty),
            withSources: true
        );

    [Fact]
    public Task DoesCreateDiagnosticIfEmitAttributeMSBuildVariableIsNotABool()
        => RunTest(
            (options, gen) => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(gen.GenerateAttributeMSBuildProperty), "ABC"))
        );

    [Fact]
    public Task DoesEmitCustomAttributeModifier()
        => RunTest(
            (options, gen) => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(gen.AttributeVisibilityMSBuildProperty), "public")),
            withSources: true
        );
}

public class XenialExpandMemberTests : AttributeGeneratorBaseTests<XenialExpandMemberAttributeGenerator>
{
    protected override XenialExpandMemberAttributeGenerator CreateTargetGenerator()
        => new XenialExpandMemberAttributeGenerator(false);
}
