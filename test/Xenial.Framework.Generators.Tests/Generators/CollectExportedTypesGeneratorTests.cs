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
public class CollectExportedTypesGeneratorTests : PartialGeneratorTest<XenialCollectExportedTypesGenerator>
{
    protected override XenialCollectExportedTypesGenerator CreateTargetGenerator() => new();

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
        => RunSourceTest("TypeList.cs",
@"using Xenial;
namespace MyProject
{
    [XenialCollectExportedTypes]
    public class MyNonPartialClass{ }
}");

    [Fact]
    public Task DoesEmitDiagnosticIfInGlobalNamespace()
        => RunSourceTest("TypeList.cs",
@"using Xenial;
[XenialCollectExportedTypes]
public partial class MyGlobalClass
{
}");

    [Fact]
    public Task DoesNotEmitDiagnosticIfPartial()
        => RunSourceTest("TypeList.cs",
@"namespace MyProject
{
    [Xenial.XenialCollectExportedTypes]
    public partial class TypeList { }
}");

    [Fact]
    public Task CollectsDomainComponent()
        => RunSourceTest("TypeList.cs",
@"namespace MyProject
{
    [DevExpress.ExpressApp.DC.DomainComponent]
    public class MyDC {}

    [Xenial.XenialCollectExportedTypes]
    public partial class TypeList { }
}");

    [Fact]
    public Task CollectsPersistentObject()
        => RunSourceTest("TypeList.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent]
    public class MyPersistentClass {}

    [Xenial.XenialCollectExportedTypes]
    public partial class TypeList { }
}");

    [Fact]
    public Task CollectsNonPersistentObject()
    => RunSourceTest("TypeList.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.NonPersistent]
    public class MyNonPersistentClass {}

    [Xenial.XenialCollectExportedTypes]
    public partial class TypeList { }
}");

    [Fact]
    public Task CollectsTypesAsExpected()
=> RunSourceTest("TypeList.cs",
@"namespace MyProject
{
    public class NormalClass { }

    [DevExpress.ExpressApp.DC.DomainDomainComponent]
    public class MyDC {}

    [DevExpress.Xpo.Persistent]
    public class MyPersistentClass {}

    [DevExpress.Xpo.NonPersistent]
    public class MyPersistentClass {}

    [Xenial.XenialCollectExportedTypes]
    public partial class TypeList { }
}");

    [Fact]
    public Task DoesNotEmitCompilerGeneratedIfExists()
=> RunSourceTest("TypeList.cs",
@"namespace MyProject
{
    [DevExpress.ExpressApp.DC.DomainComponent]
    public class MyDC { }

    [Xenial.XenialCollectExportedTypes]
    [System.Runtime.CompilerServices.CompilerGenerated]
    public partial class TypeList { }
}");
}
