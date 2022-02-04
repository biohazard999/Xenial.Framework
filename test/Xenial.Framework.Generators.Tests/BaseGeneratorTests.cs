
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using VerifyTests;

using VerifyXunit;

using Xenial.Framework.Generators.Attributes;
using Xenial.Framework.Generators.Internal;

using Xunit;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests;

public abstract class BaseGeneratorTests<TGenerator>
    where TGenerator : class, IXenialSourceGenerator
{
    protected const string CompilationName = "AssemblyName";
    protected const string XenialAttributesVisibility = "XenialAttributesVisibility";

#if FULL_FRAMEWORK || NETCOREAPP3_1
    static BaseGeneratorTests() => RegisterModuleInitializers.RegisterVerifiers();
#endif

    protected abstract TGenerator CreateTargetGenerator();

    protected virtual XenialGenerator CreateGenerator()
    {
        var generator = new XenialGenerator();
        generator.Generators.Clear();
        generator.Generators.Add(new XenialExpandMemberAttributeGenerator(false));
        generator.Generators.Add(CreateTargetGenerator());
        return generator;
    }

    protected string BuildProperty(string property)
        => $"build_property.{property}";

    protected abstract string GeneratorEmitProperty { get; }

    protected virtual IEnumerable<PortableExecutableReference> AdditionalReferences => Enumerable.Empty<PortableExecutableReference>();

    protected static CSharpCompilation CreateCompilation(IEnumerable<PortableExecutableReference> additionalReferences)
        => CSharpCompilation.Create(CompilationName,
                references: DefaultReferenceAssemblies.Concat(additionalReferences),
                //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

    protected async Task RunTest(
        Func<MockAnalyzerConfigOptionsProvider, MockAnalyzerConfigOptionsProvider>? analyzerOptions = null,
        Action<VerifySettings>? verifySettings = null,
        Func<CSharpCompilation, CSharpCompilation>? compilationOptions = null,
        Func<SyntaxTree[]>? syntaxTrees = null,
        Func<IEnumerable<AdditionalText>>? additionalTexts = null,
        string? typeToLoad = null,
        Func<TGenerator, TGenerator>? prepareGenerator = null,
        Func<CSharpCompilation>? createCompilation = null
    )
    {
        var compilation = createCompilation == null
            ? CreateCompilation(AdditionalReferences)
            : createCompilation();

        if (compilationOptions is not null)
        {
            compilation = compilationOptions(compilation);
        }

        if (syntaxTrees is not null)
        {
            compilation = compilation.AddSyntaxTrees(syntaxTrees());
        }

        var generator = CreateGenerator();

        if (prepareGenerator is not null)
        {
            var prevGenerator = generator.Generators.OfType<TGenerator>().First();
            var newGenerator = prepareGenerator.Invoke(prevGenerator);
            if (newGenerator != prevGenerator)
            {
                var index = generator.Generators.IndexOf(prevGenerator);
                generator.Generators[index] = newGenerator;
            }
        }

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
