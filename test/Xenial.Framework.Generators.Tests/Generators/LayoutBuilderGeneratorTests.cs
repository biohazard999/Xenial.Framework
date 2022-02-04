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

using Xenial.Framework.Generators.Partial;

using Xunit;

namespace Xenial.Framework.Generators.Tests.Generators;

[UsesVerify]
public class LayoutBuilderGeneratorTests : PartialGeneratorTest<XenialLayoutBuilderGenerator>
{
    protected override XenialLayoutBuilderGenerator CreateTargetGenerator()
        => new();

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
                MetadataReference.CreateFromFile(typeof(Xenial.Framework.Layouts.LayoutBuilder<>).Assembly.Location)
            }),
            Compile = false
        }, filePath);


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

    [Fact]
    public Task DoesEmitSimpleProperties()
        => RunSourceTest("DoesEmitSimpleProperties.cs",
@"using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class TargetClass
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public byte[] ByteProperty { get; set; }
        public decimal DecimalProperty { get; set; }
    }

    public partial class DoesEmitSimpleProperties : LayoutBuilder<TargetClass> { }
}");


    [Fact]
    public Task DoesEmitSimpleRelation()
        => RunSourceTest("DoesEmitSimpleRelation.cs",
@"using System.Collections.Generic;
using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class ParentClass
    {
        public IList<TargetClass> Parents { get; }
    }

    public class TargetClass
    {
        public ParentClass Parent { get; set; }
    }

    public partial class DoesEmitSimpleRelation : LayoutBuilder<TargetClass> { }
    public partial class DoesEmitSimpleRelationForParent : LayoutBuilder<ParentClass> { }
}");


    [Fact]
    public Task DoesExpandMembers()
        => RunSourceTest("DoesExpandConstants.cs",
@"using System.Collections.Generic;
using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class ParentClass
    {
        public IList<TargetClass> Parents { get; }
    }

    public class TargetClass
    {
        public ParentClass Parent { get; set; }
    }

    [XenialExpandMember(Constants.Parent)]
    public partial class TargetClassBuilder : LayoutBuilder<TargetClass> { }
}");

    [Fact]
    public Task DoesExpandComplexMemberTree()
        => RunSourceTest("DoesExpandComplexMemberTree.cs",
@"using System.Collections.Generic;
using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class GrandParentClass
    {
        public string GrandParentStringMember { get; set; }
    }

    public class ParentClass
    {
        public GrandParentClass GrandParent { get; set;}
        public IList<TargetClass> Parents { get; }
    }

    public class TargetClass
    {
        public ParentClass Parent { get; set; }
    }

    [XenialExpandMember(Constants.Parent)]
    [XenialExpandMember(Constants._Parent.GrandParent)]
    public partial class TargetClassBuilder : LayoutBuilder<TargetClass> { }
}");

    [Fact]
    public Task DoesExpandComplexMemberTree2()
        => RunSourceTest("DoesExpandComplexMemberTree2.cs",
@"using System.Collections.Generic;
using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class ParentClass
    {
        public string ParentString { get; set;}
    }

    public class TargetClass
    {
        public ParentClass Parent1 { get; set; }
        public ParentClass Parent2 { get; set; }
    }

    [XenialExpandMember(Constants.Parent1)]
    [XenialExpandMember(Constants.Parent2)]
    public partial class TargetClassBuilder : LayoutBuilder<TargetClass> { }
}");

    [Fact]
    public Task DoesExpandComplexMemberTree3()
        => RunSourceTest("DoesExpandComplexMemberTree2.cs",
@"using System.Collections.Generic;
using Xenial;
using Xenial.Framework.Layouts;

namespace MyProject
{
    public class ParentClass
    {
        public string ParentString { get; set;}
    }

    public class TargetClass
    {
        public ParentClass Parent1 { get; set; }
    }

    [XenialExpandMember(Constants.Parent1)]
    [XenialExpandMember(Constants.Parent1)]
    public partial class TargetClassBuilder : LayoutBuilder<TargetClass> { }
}");
}
