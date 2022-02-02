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

using Xenial.Framework.Generators.Base;

using Xunit;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests.Attributes;

[UsesVerify]
public abstract class AttributeGeneratorBaseTests<TGenerator>
    where TGenerator : XenialAttributeGenerator
{
    protected const string CompilationName = "AssemblyName";

#if FULL_FRAMEWORK || NETCOREAPP3_1
    static AttributeGeneratorBaseTests() => RegisterModuleInitializers.RegisterVerifiers();
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

    protected static CSharpCompilation CreateCompilation(
        Func<SyntaxTree[]>? syntaxTrees = null
    )
        => CSharpCompilation.Create(CompilationName,
                references: DefaultReferenceAssemblies,
                syntaxTrees: syntaxTrees?.Invoke(),
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

    protected async Task RunTest(
        Func<MockAnalyzerConfigOptionsProvider, TGenerator, MockAnalyzerConfigOptionsProvider>? analyzerOptions = null,
        Action<VerifySettings>? verifySettings = null,
        Func<SyntaxTree[]>? syntaxTrees = null,
        bool withSources = false,
        bool compile = true
    )
    {
        var compilation = CreateCompilation(syntaxTrees);


        var generator = CreateGenerator();

        var targetGenerator = withSources ? CreateGeneratorWithAddSources() : CreateGeneratorWithoutAddSources();

        var type = compilation.GetTypeByMetadataName(targetGenerator.AttributeFullName);

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

        if (compile)
        {
            (driver, var diagnositcs, var compilationException, var loadedType)
                = driver.CompileAndLoadType(compilation, targetGenerator.AttributeFullName);

            VerifyDiagnostics(diagnositcs, compilationException);
        }
        else
        {
            driver = driver.RunGenerators(compilation);
        }

        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        verifySettings?.Invoke(settings);
        await Verifier.Verify(driver, settings);
    }

    protected string BuildProperty(string property)
        => $"build_property.{property}";

    [Fact]
    public Task EmitAttribute()
        => RunTest(withSources: true);

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

    [Fact]
    public Task DoesNotEmitIfAttributeExist()
        => RunTest(
            syntaxTrees: () =>
            {
                var generator = CreateTargetGenerator();

                return new[]
                {
                    BuildSyntaxTree(generator.AttributeName, @$"namespace {generator.AttributeNamespace}
{{
    public sealed class {generator.AttributeName} : System.Attribute {{ }}
}}")
                };
            },
            withSources: true
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
