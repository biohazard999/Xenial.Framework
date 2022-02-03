
using System;

using System.Linq;
using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.Generators.Tests.Base;

using Xunit;

namespace Xenial.Framework.Generators.Tests.Attributes;

public record AttributeGeneratorTestOptions<TTargetGenerator> : GeneratorTestOptionsBase<TTargetGenerator>
    where TTargetGenerator : XenialAttributeGenerator
{
    public AttributeGeneratorTestOptions(Func<TTargetGenerator> createTargetGenerator) : base(createTargetGenerator)
    {
    }
}

[UsesVerify]
public abstract class AttributeGeneratorBaseTests<TGenerator>
    where TGenerator : XenialAttributeGenerator
{

#if FULL_FRAMEWORK || NETCOREAPP3_1
    static AttributeGeneratorBaseTests() => RegisterModuleInitializers.RegisterVerifiers();
#endif
    protected abstract TGenerator CreateTargetGenerator();

    protected TGenerator CreateGeneratorWithoutAddSources()
        => CreateTargetGenerator() with { AddSource = false };

    protected TGenerator CreateGeneratorWithAddSources()
        => CreateTargetGenerator() with { AddSource = true };

    internal async Task RunTest(Func<AttributeGeneratorTestOptions<TGenerator>, AttributeGeneratorTestOptions<TGenerator>>? configureOptions = null)
    {
        var options = new AttributeGeneratorTestOptions<TGenerator>(CreateGeneratorWithAddSources);

        if (configureOptions is not null)
        {
            options = configureOptions(options);
        }

        options = options with
        {
            TargetGenerator = options.AddSources ? CreateGeneratorWithAddSources() : CreateGeneratorWithoutAddSources()
        };

        options = options with
        {
            CreateGenerator = (o) =>
            {
                var generator = GeneratorTestOptionsBase.EmptyGenerator(o);

                generator.Generators.Add(
                    options.TargetGenerator
                );
                return generator;
            }
        };

        if (options.Compile)
        {
            BaseGeneratorTest.Compile((o) => options);
        }

        await BaseGeneratorTest.RunTest((o) => options);
    }

    [Fact]
    public Task EmitAttribute()
        => RunTest();

    //[Theory]
    //TODO: Make it possible to opt out of all Attributes with one MSBuildProperty
    //[InlineData(true)]
    //[InlineData(false)]
    //public Task DoesEmitIfGenerateXenialAttributesMsBuildProperty(bool emitProperty)
    //    => RunTest(
    //        (options, gen) => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(gen.GenerateAttributeMSBuildProperty), emitProperty.ToString())),
    //        settings => settings.UseParameters(emitProperty),
    //        withSources: true
    //    );

    //[Fact]
    //TODO: Make it possible to opt out of all Attributes with one MSBuildProperty
    //public Task DoesCreateDiagnosticIfGenerateXenialAttributesMsBuildPropertyIsNotABool()
    //    => RunTest(
    //        (options, gen) => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(gen.GenerateAttributeMSBuildProperty), "ABC")),
    //        withSources: true
    //    );

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DoesEmitIfSpecificMsBuildProperty(bool emitProperty)
        => RunTest(options => options with
        {
            MockOptions = (o) => o.MockOptionsProvider.WithGlobalOptions(new MockAnalyzerConfigOptions(o.BuildProperty(options.TargetGenerator.GenerateAttributeMSBuildProperty), emitProperty.ToString())),
            VerifySettings = (o, settings) => settings.UseParameters(emitProperty),
            AddSources = true
        });

    [Fact]
    public Task DoesCreateDiagnosticIfEmitAttributeMSBuildVariableIsNotABool()
        => RunTest(options => options with
        {
            MockOptions = o => o.MockOptionsProvider.WithGlobalOptions(new MockAnalyzerConfigOptions(o.BuildProperty(options.TargetGenerator.GenerateAttributeMSBuildProperty), "ABC")),
            Compile = false
        });

    [Fact]
    public Task DoesEmitCustomAttributeModifier()
        => RunTest(options => options with
        {
            MockOptions = o => o.MockOptionsProvider.WithGlobalOptions(new MockAnalyzerConfigOptions(o.BuildProperty(options.TargetGenerator.AttributeVisibilityMSBuildProperty), "public")),
            AddSources = true
        });

    [Fact]
    public Task DoesNotEmitIfAttributeExist()
        => RunTest(options => options with
        {
            SyntaxTrees = o => new[]
            {
                o.BuildSyntaxTree(options.TargetGenerator.AttributeName,@$"namespace {options.TargetGenerator.AttributeNamespace}
{{
    public sealed class {options.TargetGenerator.AttributeName} : System.Attribute {{ }}
}}")
            },
            AddSources = true
        });
}
