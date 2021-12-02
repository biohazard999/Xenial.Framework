using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ActionsGeneratorTests : BaseGeneratorTests<XenialActionGenerator>
{
    protected override string GeneratorEmitProperty => XenialActionGenerator.GenerateXenialActionAttributeMSBuildProperty;

    protected Task RunSourceTest(string fileName, string source)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialActionsAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            });

    [Fact]
    public Task WarnsIfClassIsNotPartial()
        => RunSourceTest("ClassShouldBePartial",
@"namespace MyActions
{
    [Xenial.XenialAction]
    public class ClassShouldBePartial { }
}");
}

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialActionsAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialActionGenerator.GenerateXenialActionsAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
