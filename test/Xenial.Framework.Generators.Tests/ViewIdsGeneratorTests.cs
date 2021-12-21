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

using Xenial.Framework.Base;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ViewIdsGeneratorTests : BaseGeneratorTests<XenialViewIdsGenerator>
{
    protected override XenialViewIdsGenerator CreateTargetGenerator() => new();

    protected override string GeneratorEmitProperty => XenialViewIdsGenerator.GenerateXenialViewIdsAttributeMSBuildProperty;

    protected override IEnumerable<PortableExecutableReference> AdditionalReferences
    {
        get
        {
            yield return MetadataReference.CreateFromFile(typeof(DomainComponentAttribute).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(PersistentAttribute).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(GenerateNoDetailViewAttribute).Assembly.Location);
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

internal static partial class CompilationHelpers
{
    public static CSharpCompilation AddInlineXenialViewIdsAttribute(this CSharpCompilation compilation, string visibility = "internal")
    {
        (_, var syntaxTree) = XenialViewIdsGenerator.GenerateXenialViewIdsAttribute(visibility: visibility);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}
