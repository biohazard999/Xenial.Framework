
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using System.Linq;
using System.Threading.Tasks;

using VerifyTests;

using VerifyXunit;

using Xenial.Framework.Generators.Base;

using Xunit;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests.Attributes;

public record GeneratorTestOptions
{
    public string CompilationName { get; set; } = "AssemblyName";
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

    public Func<GeneratorTestOptions, CSharpCompilation> CreateCompilation { get; set; } = options
        => CSharpCompilation.Create(options.CompilationName,
            references: options.ReferenceAssemblies,
            syntaxTrees: options.SyntaxTrees?.Invoke(options),
            //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

    public Func<GeneratorTestOptions, XenialGenerator> CreateGenerator { get; set; }
        = o => new();

    public static Func<GeneratorTestOptions, XenialGenerator> EmptyGenerator { get; } = (o) =>
    {
        var generator = new XenialGenerator();
        generator.Generators.Clear();
        return generator;
    };

    public IEnumerable<PortableExecutableReference> ReferenceAssemblies { get; set; }
        = DefaultReferenceAssemblies;

    public Func<GeneratorTestOptions, IEnumerable<SyntaxTree>>? SyntaxTrees { get; set; }

    public MockAnalyzerConfigOptionsProvider MockOptionsProvider { get; set; }
        = MockAnalyzerConfigOptionsProvider.Empty;

    public Func<GeneratorTestOptions, MockAnalyzerConfigOptionsProvider>? MockOptions { get; set; }

    public Func<GeneratorTestOptions, XenialGenerator, GeneratorDriver> CreateDriver { get; set; }
        = (options, generator)
            => CSharpGeneratorDriver.Create(
                new[] { generator },
                optionsProvider: options.MockOptionsProvider
            );

    public string BuildProperty(string property)
        => $"build_property.{property}";

    public SyntaxTree BuildSyntaxTree(string fileName, string sourceText)
    {
        var syntaxTree =
            CSharpSyntaxTree.ParseText(
                sourceText,
                new CSharpParseOptions(LanguageVersion),
                path: fileName
            );

        return syntaxTree;
    }

    public Action<GeneratorTestOptions, VerifySettings>? VerifySettings { get; set; }

    public bool Compile { get; set; } = true;
    public bool AddSources { get; set; } = true;
}

public record GeneratorTestOptions<TTargetGenerator> : GeneratorTestOptions
    where TTargetGenerator : class
{
    public GeneratorTestOptions(Func<TTargetGenerator> createTargetGenerator)
        => TargetGenerator = createTargetGenerator();

    public TTargetGenerator TargetGenerator { get; set; }
}

public record AttributeGeneratorTestOptions<TTargetGenerator> : GeneratorTestOptions<TTargetGenerator>
    where TTargetGenerator : XenialAttributeGenerator
{
    public AttributeGeneratorTestOptions(Func<TTargetGenerator> createTargetGenerator) : base(createTargetGenerator)
    {
    }
}

internal class GeneratorTest
{
    public static (
        GeneratorTestOptions options,
        Compilation compilation,
        XenialGenerator generator,
        GeneratorDriver driver
    ) PrepareTest(Func<GeneratorTestOptions, GeneratorTestOptions> optionsFunctions)
    {
        var options = optionsFunctions(new());
        var compilation = options.CreateCompilation(options);
        var generator = options.CreateGenerator(options);

        options = options with
        {
            MockOptionsProvider = options.MockOptions is not null
                ? options.MockOptions(options)
                : options.MockOptionsProvider
        };

        var driver = options.CreateDriver(options, generator);
        return (options, compilation, generator, driver);
    }

    public static async Task RunTest(Func<GeneratorTestOptions, GeneratorTestOptions> optionsFunctions)
    {
        var (options, compilation, generator, driver) = PrepareTest(optionsFunctions);

        driver = driver.RunGenerators(compilation);

        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        options.VerifySettings?.Invoke(options, settings);
        await Verifier.Verify(driver, settings);
    }

    public static void Compile(Func<GeneratorTestOptions, GeneratorTestOptions> optionsFunctions)
    {
        var (options, compilation, generator, driver) = PrepareTest(optionsFunctions);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var diagnositcs);

        VerifyDiagnostics(diagnositcs, null);
    }

    private static void VerifyDiagnostics(ImmutableArray<Diagnostic> diagnostics, Exception? ex)
    {
        if (diagnostics.Length > 0)
        {
            var argumentException = new ArgumentException(string.Join(
                Environment.NewLine,
                diagnostics.Select(diag => new DiagnosticFormatter().Format(diag))
            ));

            throw ex is null
                ? new AggregateException(argumentException)
                : new AggregateException(argumentException, ex);
        }
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
                var generator = GeneratorTestOptions.EmptyGenerator(o);

                generator.Generators.Add(
                    options.TargetGenerator
                );
                return generator;
            }
        };

        if (options.Compile)
        {
            GeneratorTest.Compile((o) => options);
        }

        await GeneratorTest.RunTest((o) => options);
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
