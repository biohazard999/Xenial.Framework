using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using VerifyXunit;

using Xenial.Framework.Base;
using Xenial.Framework.Generators.Partial;

using Xunit;

namespace Xenial.Framework.Generators.Tests.Generators;

[UsesVerify]
public class CollectControllersGeneratorTests : PartialGeneratorTest<XenialCollectControllersGenerator>
{
    protected override XenialCollectControllersGenerator CreateTargetGenerator() => new();

    public override Task RunSourceTest(string fileName, string source, [CallerFilePath] string filePath = "")
        => RunTest(o => o with
        {
            SyntaxTrees = o => new[]
            {
                o.BuildSyntaxTree(fileName, source)
            },
            ReferenceAssembliesProvider = o => o.ReferenceAssemblies.Concat(new[]
            {
                MetadataReference.CreateFromFile(typeof(DomainComponentAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(PersistentAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(GenerateNoDetailViewAttribute).Assembly.Location)
            }),
            Compile = false
        }, filePath);

    [Fact]
    public Task DoesEmitDiagnosticIfNotPartial()
        => RunSourceTest("ControllerTypeList.cs",
@"using Xenial;
namespace MyProject
{
    [XenialCollectControllers]
    public class MyNonPartialClass{ }
}");

    [Fact]
    public Task DoesEmitDiagnosticIfInGlobalNamespace()
        => RunSourceTest("ControllerTypeList.cs",
@"using Xenial;
[XenialCollectControllers]
public partial class MyGlobalClass
{
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfPartial()
        => RunSourceTest("ControllerTypeList.cs",
@"namespace MyProject
{
    [Xenial.XenialCollectControllers]
    public partial class ControllerTypeList { }
}");

    [Fact]
    public Task CollectsBasicController()
        => RunSourceTest("ControllerTypeList.cs",
@"namespace MyProject
{
    public class MyController : DevExpress.ExpressApp.Controller
    { }

    [Xenial.XenialCollectControllers]
    public partial class ControllerTypeList { }
}");

    [Fact]
    public Task CollectsBasicViewController()
        => RunSourceTest("ControllerTypeList.cs",
@"namespace MyProject
{
    public class MyController : DevExpress.ExpressApp.ViewController
    { }

    [Xenial.ControllerTypeList]
    public partial class ControllerTypeList { }
}");

}
