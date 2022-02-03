
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;

using VerifyTests;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests.Base;

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
        => TargetGenerator = createTargetGenerator?.Invoke()!;

    public TTargetGenerator TargetGenerator { get; set; }
}
