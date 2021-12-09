﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using VerifyXunit;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ViewIdsGeneratorTests : BaseGeneratorTests<XenialViewIdsGenerator>
{
    protected override XenialViewIdsGenerator CreateTargetGenerator() => new();

    protected override string GeneratorEmitProperty => XenialViewIdsGenerator.GenerateXenialViewIdsAttributeMSBuildProperty;

    protected override IEnumerable<Microsoft.CodeAnalysis.PortableExecutableReference> AdditionalReferences
    {
        get
        {
            yield return MetadataReference.CreateFromFile(typeof(DomainComponentAttribute).Assembly.Location);
        }
    }

    protected Task RunSourceTest(string fileName, string source)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialViewIdsAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            });

    [Fact]
    public Task DoesEmitDiagnosticIfNotPartial()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
namespace MyProject
{
    [XenialViewIds]
    public class MyNonPartialClass{ }
}");

    [Fact]
    public Task DoesEmitDiagnosticIfInGlobalNamespace()
        => RunSourceTest("MyNonPartialClass.cs",
@"using Xenial;
[XenialViewIds]
public partial class MyGlobalClass
{
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfPartial()
        => RunSourceTest("MyPartialClass.cs",
@"namespace MyProject
{
    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task CollectsBasicDomainComponent()
    => RunSourceTest("MyPartialClass.cs",
@"namespace MyProject
{
    [DevExpress.ExpressApp.DC.DomainComponent]
    public class DomainComponent { }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");
}

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialViewIdsAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialViewIdsGenerator.GenerateXenialViewIdsAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
