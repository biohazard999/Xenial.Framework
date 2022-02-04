using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.Generators.Tests.Base;

namespace Xenial.Framework.Generators.Tests.Generators;


public record PartialGeneratorTestOptions<TTargetGenerator> : GeneratorTestOptionsBase<TTargetGenerator>
    where TTargetGenerator : XenialPartialGenerator
{
    public PartialGeneratorTestOptions(Func<TTargetGenerator> createTargetGenerator) : base(createTargetGenerator)
    {
    }

    public Func<TTargetGenerator, TTargetGenerator> PrepareGenerator { get; set; } = g => g;
}


public abstract class PartialGeneratorTest<TGenerator>
    where TGenerator : XenialPartialGenerator
{

#if FULL_FRAMEWORK || NETCOREAPP3_1
    static PartialGeneratorTest() => RegisterModuleInitializers.RegisterVerifiers();
#endif
    protected abstract TGenerator CreateTargetGenerator();

    protected TGenerator CreateGeneratorWithoutAddSources()
        => CreateTargetGenerator() with { AddSource = false };

    protected TGenerator CreateGeneratorWithAddSources()
        => CreateTargetGenerator() with { AddSource = true };

    internal async Task RunTest(
        Func<PartialGeneratorTestOptions<TGenerator>, PartialGeneratorTestOptions<TGenerator>>? configureOptions = null,
        [CallerFilePath] string filePath = ""
    )
    {
        var options = new PartialGeneratorTestOptions<TGenerator>(CreateGeneratorWithAddSources);

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
            TargetGenerator = options.PrepareGenerator(options.TargetGenerator)
        };

        options = options with
        {
            CreateGenerator = (o) =>
            {
                var generator = GeneratorTestOptionsBase.EmptyGenerator(o);

                foreach (var dependendGenerator in options.TargetGenerator.DependsOnGenerators)
                {
                    generator.Generators.Add(dependendGenerator with
                    {
                        AddSource = false
                    });
                }

                generator.Generators.Add(
                    options.TargetGenerator
                );
                return generator;
            }
        };

        var additionalFiles = options.AdditionalFiles.ToList();

        var optionsProvider = options.MockOptionsProvider;

        foreach (var additionalFile in additionalFiles)
        {
            optionsProvider = optionsProvider.WithAdditionalTreeOptions(
                additionalFile.Files.ToImmutableDictionary(k => (object)k, _ => (AnalyzerConfigOptions)new MockAnalyzerConfigOptions($"build_metadata.AdditionalFiles.{additionalFile.Key}", "true"))
            );
        }

        options = options with
        {
            MockOptionsProvider = optionsProvider
        };


        if (options.Compile)
        {
            BaseGeneratorTest.Compile((o) => options);
        }

        await BaseGeneratorTest.RunTest((o) => options, filePath);
    }

    public virtual Task RunSourceTest(string fileName, string source, [CallerFilePath] string filePath = "")
        => RunTest(o => o with
        {
            SyntaxTrees = o => new[]
            {
                o.BuildSyntaxTree(fileName, source)
            },
            Compile = false
        }, filePath);

    public virtual Task RunSourceTestWithAdditionalFiles(string fileName, string source, List<AdditionalFiles> additionalFiles, [CallerFilePath] string filePath = "")
        => RunTest(options => options with
        {
            SyntaxTrees = o => new[]
            {
                o.BuildSyntaxTree(fileName, source)
            },
            AdditionalFiles = additionalFiles,
            Compile = false
        }, filePath);

}
