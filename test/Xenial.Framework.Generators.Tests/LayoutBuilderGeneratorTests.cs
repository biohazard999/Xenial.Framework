using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class LayoutBuilderGeneratorTests : BaseGeneratorTests<XenialLayoutBuilderGenerator>
{
    protected override XenialLayoutBuilderGenerator CreateTargetGenerator() => new();

    protected override string GeneratorEmitProperty => XenialLayoutBuilderGenerator.GenerateXenialLayoutBuilderAttributeMSBuildProperty;

    protected override IEnumerable<PortableExecutableReference> AdditionalReferences
    {
        get
        {
            yield return MetadataReference.CreateFromFile(typeof(DomainComponentAttribute).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(PersistentAttribute).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(Xenial.Framework.Layouts.LayoutBuilder<>).Assembly.Location);
        }
    }

    protected Task RunSourceTest(string fileName, string source)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialLayoutBuilderAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            });

    [Fact]
    public Task DoesEmitDiagnosticIfNotPartial()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class TargetClass { }

    public class MyNonPartialClass : LayoutBuilder<TargetClass> { }
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfPartial()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class TargetClass { }

    public partial class MyPartialClass : LayoutBuilder<TargetClass> { }
}");
}

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialLayoutBuilderAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialLayoutBuilderGenerator.GenerateXenialLayoutBuilderAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
