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
public class XpoBuilderGeneratorTests : BaseGeneratorTests<XenialXpoBuilderGenerator>
{
    protected override XenialXpoBuilderGenerator CreateTargetGenerator() => new();

    protected override string GeneratorEmitProperty => XenialXpoBuilderGenerator.GenerateXenialXpoBuilderAttributeMSBuildProperty;

    protected override IEnumerable<PortableExecutableReference> AdditionalReferences
    {
        get
        {
            yield return MetadataReference.CreateFromFile(typeof(DomainComponentAttribute).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(PersistentAttribute).Assembly.Location);
        }
    }

    protected Task RunSourceTest(string fileName, string source)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialXpoBuilderAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            });

    [Fact]
    public Task DoesEmitBasicBuilder()
        => RunSourceTest("MyNormalClassWithoutCtor.cs",
@"using Xenial;
namespace MyProject
{
    [XenialXpoBuilder]
    public class MyNormalClassWithoutCtor { }
}");

    [Fact]
    public Task BasicStringProperty()
        => RunSourceTest("BasicStringProperty.cs",
@"using Xenial;
namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicStringProperty
    {
        public string StringProperty { get; set; }
        public System.String StringProperty2 { get; set; }
    }
}");

    [Fact]
    public Task BasicIntProperty()
        => RunSourceTest("BasicIntProperty.cs",
@"using Xenial;
namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicIntProperty
    {
        public int IntProperty { get; set; }
        public System.Int32 IntProperty2 { get; set; }
        public System.Int64 IntProperty3 { get; set; }
    }
}");

    [Fact]
    public Task BasicBoolProperty()
        => RunSourceTest("BasicBoolProperty.cs",
@"using Xenial;
namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicBoolProperty
    {
        public bool BoolProperty { get; set; }
        public System.Boolean BoolProperty2 { get; set; }
    }
}");

    [Fact]
    public Task BasicDecimalProperty()
        => RunSourceTest("BasicDecimalProperty.cs",
@"using Xenial;
namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicDecimalProperty
    {
        public decimal DecimalProperty { get; set; }
        public System.Decimal DecimalProperty2 { get; set; }
    }
}");

    [Fact]
    public Task BasicDoubleProperty()
        => RunSourceTest("BasicDoubleProperty.cs",
@"using Xenial;
namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicDoubleProperty
    {
        public double DoubleProperty { get; set; }
        public System.Double DoubleProperty2 { get; set; }
    }
}");

    [Fact]
    public Task BasicXpoCtorTest()
        => RunSourceTest("BasicXpoCtorTest.cs",
@"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpoCtorObject : XPObject
    {
        public BasicXpoCtorObject(Session session) : base(session) { }
    }
}");
}

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialXpoBuilderAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialXpoBuilderGenerator.GenerateXenialXpoBuilderAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
