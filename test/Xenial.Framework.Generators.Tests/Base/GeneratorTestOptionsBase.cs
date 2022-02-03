
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Linq;

using VerifyTests;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests.Base;

public record GeneratorTestOptionsBase
{
    public string CompilationName { get; set; } = "AssemblyName";
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

    public Func<GeneratorTestOptionsBase, CSharpCompilation> CreateCompilation { get; set; } = options
        => CSharpCompilation.Create(options.CompilationName,
            references: options.ReferenceAssemblies,
            syntaxTrees: options.SyntaxTrees?.Invoke(options),
            //It's necessary to output as a DLL in order to get the compiler in a cooperative mood. 
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

    public Func<GeneratorTestOptionsBase, XenialGenerator> CreateGenerator { get; set; }
        = o => new();

    public Func<GeneratorTestOptionsBase, IEnumerable<AdditionalFiles>> AdditionalFiles { get; set; }
        = (o) => Enumerable.Empty<AdditionalFiles>();

    public static Func<GeneratorTestOptionsBase, XenialGenerator> EmptyGenerator { get; } = (o) =>
    {
        var generator = new XenialGenerator();
        generator.Generators.Clear();
        return generator;
    };

    public IEnumerable<PortableExecutableReference> ReferenceAssemblies { get; set; }
        = DefaultReferenceAssemblies;

    public Func<GeneratorTestOptionsBase, IEnumerable<SyntaxTree>>? SyntaxTrees { get; set; }

    public MockAnalyzerConfigOptionsProvider MockOptionsProvider { get; set; }
        = MockAnalyzerConfigOptionsProvider.Empty;

    public Func<GeneratorTestOptionsBase, MockAnalyzerConfigOptionsProvider>? MockOptions { get; set; }

    public Func<IEnumerable<AdditionalText>> AdditionalTexts { get; set; } = () => Enumerable.Empty<AdditionalText>();

    public Func<GeneratorTestOptionsBase, XenialGenerator, GeneratorDriver> CreateDriver { get; set; }
        = (options, generator)
            => CSharpGeneratorDriver.Create(
                new[] { generator },
                optionsProvider: options.MockOptionsProvider,
                additionalTexts: options.AdditionalTexts()
            );

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
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

    public Action<GeneratorTestOptionsBase, VerifySettings>? VerifySettings { get; set; }

    public bool Compile { get; set; } = true;
    public bool AddSources { get; set; } = true;
}

public record GeneratorTestOptionsBase<TTargetGenerator> : GeneratorTestOptionsBase
    where TTargetGenerator : class
{
    public GeneratorTestOptionsBase(Func<TTargetGenerator> createTargetGenerator)
        => TargetGenerator = createTargetGenerator?.Invoke()!;

    public TTargetGenerator TargetGenerator { get; set; }
}


[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "<Pending>")]
public record AdditionalFiles(string Key, List<MockAdditionalText> Files);
