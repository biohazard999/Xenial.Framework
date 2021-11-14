using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xenial.Framework.Generators.Tests;

internal sealed class MockAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
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
        => treeDict.TryGetValue(tree, out var options) ? options : MockAnalyzerConfigOptions.Empty;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        => treeDict.TryGetValue(textFile, out var options) ? options : MockAnalyzerConfigOptions.Empty;

    internal MockAnalyzerConfigOptionsProvider WithAdditionalTreeOptions(ImmutableDictionary<object, AnalyzerConfigOptions> treeDict)
        => new MockAnalyzerConfigOptionsProvider(this.treeDict.AddRange(treeDict), GlobalOptions);

    internal MockAnalyzerConfigOptionsProvider WithGlobalOptions(AnalyzerConfigOptions globalOptions)
        => new MockAnalyzerConfigOptionsProvider(treeDict, globalOptions);
}

