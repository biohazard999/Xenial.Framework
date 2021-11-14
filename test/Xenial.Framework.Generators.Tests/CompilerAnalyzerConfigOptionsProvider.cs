using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xenial.Framework.Generators.Tests;

internal sealed class CompilerAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly ImmutableDictionary<object, AnalyzerConfigOptions> treeDict;

    public static CompilerAnalyzerConfigOptionsProvider Empty { get; }
        = new CompilerAnalyzerConfigOptionsProvider(
            ImmutableDictionary<object, AnalyzerConfigOptions>.Empty,
            CompilerAnalyzerConfigOptions.Empty);

    internal CompilerAnalyzerConfigOptionsProvider(
        ImmutableDictionary<object, AnalyzerConfigOptions> treeDict,
        AnalyzerConfigOptions globalOptions)
    {
        this.treeDict = treeDict;
        GlobalOptions = globalOptions;
    }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        => treeDict.TryGetValue(tree, out var options) ? options : CompilerAnalyzerConfigOptions.Empty;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        => treeDict.TryGetValue(textFile, out var options) ? options : CompilerAnalyzerConfigOptions.Empty;

    internal CompilerAnalyzerConfigOptionsProvider WithAdditionalTreeOptions(ImmutableDictionary<object, AnalyzerConfigOptions> treeDict)
        => new CompilerAnalyzerConfigOptionsProvider(this.treeDict.AddRange(treeDict), GlobalOptions);

    internal CompilerAnalyzerConfigOptionsProvider WithGlobalOptions(AnalyzerConfigOptions globalOptions)
        => new CompilerAnalyzerConfigOptionsProvider(treeDict, globalOptions);
}

