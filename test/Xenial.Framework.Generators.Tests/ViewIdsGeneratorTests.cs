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
using Xenial.Framework.Generators.Tests.Generators;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ViewIdsGeneratorTests : PartialGeneratorTest<XenialViewIdsGenerator>
{
    protected override XenialViewIdsGenerator CreateTargetGenerator() => new();

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

    [Fact]
    public Task CollectsBasicPersistentType()
        => RunSourceTest("MyPartialClass.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent(""MyPersistent"")]
    public class PersistentObject { }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task CollectsBasicNonPersistentType()
        => RunSourceTest("MyPartialClass.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.NonPersistent]
    public class NonPersistentObject { }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task CollectsXPCollection()
        => RunSourceTest("CollectsXPCollection.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent]
    public class PersistentChildObject { }

    [DevExpress.Xpo.Persistent]
    public class PersistentObject
    {
        public DevExpress.Xpo.XPCollection<PersistentChildObject> Children
            => throw null;
    }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");


    [Fact]
    public Task CollectsManyToMany()
        => RunSourceTest("CollectsManyToMany.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent]
    public class PersistentChildObject
    {
        public DevExpress.Xpo.XPCollection<PersistentObject> Parents
            => throw null;
    }

    [DevExpress.Xpo.Persistent]
    public class PersistentObject
    {
        public DevExpress.Xpo.XPCollection<PersistentChildObject> Children
            => throw null;
    }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task RemovesDetailView()
        => RunSourceTest("RemovesDetailView.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent]
    [Xenial.Framework.Base.GenerateNoDetailView]
    public class PersistentObject
    {
    }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task RemovesListView()
        => RunSourceTest("RemovesListView.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent]
    [Xenial.Framework.Base.GenerateNoListView]
    public class PersistentObject
    {
    }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task RemovesLookupListView()
        => RunSourceTest("RemovesLookupListView.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent]
    [Xenial.Framework.Base.GenerateNoLookupListView]
    public class PersistentObject
    {
    }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task RemovesNestedListView()
        => RunSourceTest("RemovesNestedListView.cs",
@"namespace MyProject
{
    [DevExpress.Xpo.Persistent]
    public class PersistentChildObject { }

    [DevExpress.Xpo.Persistent]
    [Xenial.Framework.Base.GenerateNoNestedListView(nameof(Children))]
    public class PersistentObject
    {
        public DevExpress.Xpo.XPCollection<PersistentChildObject> Children
            => throw null;
    }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");

    [Fact]
    public Task DeclareCustomViews()
    => RunSourceTest("DeclareDetailView.cs",
@"using DevExpress.Xpo;
using Xenial.Framework.Base;

namespace MyProject
{
    [Persistent]
    [DeclareDetailView(""PersistentObject_Custom1_DetailView"")]
    [DeclareDetailView(""PersistentObject_Custom2_DetailView"")]
    [DeclareListView(""PersistentObject_Custom1_ListView"")]
    [DeclareListView(""PersistentObject_Custom1_ListView"")]
    [DeclareListView(""PersistentObject_Custom2_ListView"")]
    [DeclareDashboardView(""PersistentObject_DashboardView"")]
    public class PersistentObject
    {
    }

    [Xenial.XenialViewIds]
    public partial class MyPartialClass { }
}");
}
