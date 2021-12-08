using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class XenialViewIdsGeneratorTests : BaseGeneratorTests<XenialViewIdsGenerator>
{
    protected override XenialViewIdsGenerator CreateTargetGenerator() => new();

    protected override string GeneratorEmitProperty => XenialViewIdsGenerator.GenerateXenialViewIdsAttributeMSBuildProperty;

    protected Task RunSourceTest(string fileName, string source)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialViewIdsAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            });

}

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialViewIdsAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialViewIdsGenerator.GenerateXenialViewIdsAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
