using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xenial.Framework.Generators.Tests;

public sealed class MockAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly ImmutableDictionary<object, AnalyzerConfigOptions> treeDict;

    public static MockAnalyzerConfigOptionsProvider Empty { get; }
        = new MockAnalyzerConfigOptionsProvider(
            ImmutableDictionary<object, AnalyzerConfigOptions>.Empty,
            MockAnalyzerConfigOptions.Empty);

    internal MockAnalyzerConfigOptionsProvider(
        ImmutableDictionary<object, AnalyzerConfigOptions> treeDict,
        AnalyzerConfigOptions globalOptions)
    {
        this.treeDict = treeDict;
        GlobalOptions = globalOptions;
    }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        if (treeDict.TryGetValue(tree, out var options))
        {
            return options;
        }
        else
        {
            return MockAnalyzerConfigOptions.Empty;
        }
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        if (treeDict.TryGetValue(textFile, out var options))
        {
            return options;
        }

        var keys = treeDict.Keys.OfType<AdditionalText>();
        var key = keys.FirstOrDefault(key => key.Path == textFile.Path);
        if (key is not null)
        {
            if (treeDict.TryGetValue(key, out options))
            {
                return options;
            }
        }

        return MockAnalyzerConfigOptions.Empty;
    }

    internal MockAnalyzerConfigOptionsProvider WithAdditionalTreeOptions(ImmutableDictionary<object, AnalyzerConfigOptions> treeDict)
        => new MockAnalyzerConfigOptionsProvider(this.treeDict.AddRange(treeDict), GlobalOptions);

    internal MockAnalyzerConfigOptionsProvider WithGlobalOptions(AnalyzerConfigOptions globalOptions)
        => new MockAnalyzerConfigOptionsProvider(treeDict, globalOptions);
}

