﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
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
            yield return MetadataReference.CreateFromFile(typeof(PersistentAttribute).Assembly.Location);
        }
    }

    protected Task RunSourceTest(string fileName, string source, Func<CSharpCompilation>? createCompilation = null)
        => RunTest(
            options => options.WithGlobalOptions(new MockAnalyzerConfigOptions(BuildProperty(GeneratorEmitProperty), "false")),
            compilationOptions: compilation => compilation.AddInlineXenialXpoBuilderAttribute(),
            syntaxTrees: () => new[]
            {
                BuildSyntaxTree(fileName, source)
            },
            createCompilation: createCompilation);

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
        public BasicXpoCtorObject(Session session)
            : base(session) { }
    }
}");

    [Fact]
    public Task BasicXpoWithObjectSpaceTest()
        => RunSourceTest("BasicXpoWithObjectSpaceTest.cs",
@"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpoWithObjectSpace : XPObject
    {
        public BasicXpoWithObjectSpace(Session session)
            : base(session) { }
    }
}", () => CreateCompilation(AdditionalReferences.Concat(new[]
{
    MetadataReference.CreateFromFile(typeof(IObjectSpace).Assembly.Location)
})));

    [Fact]
    public Task BasicXpoWithAutoGeneratedNamedIntKey()
        => RunSourceTest("BasicXpoWithAutoGeneratedNamedIntKey.cs",
@"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpoWithAutoGeneratedNamedIntKey : XPLiteObject
    {
        public BasicXpoWithAutoGeneratedNamedIntKey(Session session)
            : base(session) { }

        [Key(AutoGenerate = true)]
        public int KeyProperty { get; set; }

        public string OwnStringProperty { get; set;}
    }
}");

    [Fact]
    public Task BasicXpoWithNotAutoGeneratedNamedIntKey()
        => RunSourceTest("BasicXpoWithNotAutoGeneratedNamedIntKey.cs",
@"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpoWithNotAutoGeneratedNamedIntKey : XPLiteObject
    {
        public BasicXpoWithNotAutoGeneratedNamedIntKey(Session session)
            : base(session) { }

        [Key(AutoGenerate = false)]
        public int KeyProperty { get; set; }

        public string OwnStringProperty { get; set;}
    }
}");

    [Fact]
    public Task BasicXpoWithAutoGeneratedCtorIntKey()
        => RunSourceTest("BasicXpoWithAutoGeneratedCtorIntKey.cs",
@"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpoWithAutoGeneratedCtorIntKey : XPLiteObject
    {
        public BasicXpoWithAutoGeneratedCtorIntKey(Session session)
            : base(session) { }

        [Key(true)]
        public int KeyProperty { get; set; }

        public string OwnStringProperty { get; set;}
    }
}");

    [Fact]
    public Task BasicXpoWithNotGeneratedCtorIntKey()
    => RunSourceTest("BasicXpoWithNotGeneratedCtorIntKey.cs",
@"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpoWithNotGeneratedCtorIntKey : XPLiteObject
    {
        public BasicXpoWithNotGeneratedCtorIntKey(Session session)
            : base(session) { }

        [Key(false)]
        public int KeyProperty { get; set; }

        public string OwnStringProperty { get; set;}
    }
}");

    [Fact]
    public Task BasicXpoWithAutoGeneratedDefaultIntKey()
        => RunSourceTest("BasicXpoWithAutoGeneratedNamedIntKey.cs",
@"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpoWithAutoGeneratedNamedIntKey : XPLiteObject
    {
        public BasicXpoWithAutoGeneratedNamedIntKey(Session session)
            : base(session) { }

        [Key]
        public int KeyProperty { get; set; }

        public string OwnStringProperty { get; set;}
    }
}");

    [Fact]
    public Task BasicXpoWithoutParentBuilder()
        => RunSourceTest("BasicXpoWithoutParentBuilder.cs",
    @"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    public class BasicXpoParent : XPObject
    {
        public BasicXpoParent(Session session)
            : base(session) { }

        public string ParentStringProperty { get; set;}
    }

    [XenialXpoBuilder]
    public class BasicXpoWithoutParentBuilder : BasicXpoParent
    {
        public BasicXpoWithoutParentBuilder(Session session)
            : base(session) { }

        public string OwnStringProperty { get; set;}
    }
}");


    [Fact]
    public Task BasicXpoWithoutParentsBuilders()
        => RunSourceTest("BasicXpoWithoutParentsBuilders.cs",
    @"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    public class BasicXpoGrantParent : XPObject
    {
        public BasicXpoGrantParent(Session session)
            : base(session) { }

        public string GrandParentStringProperty { get; set;}
    }

    public class BasicXpoParent : BasicXpoGrantParent
    {
        public BasicXpoParent(Session session)
            : base(session) { }

        public string ParentStringProperty { get; set;}
    }

    [XenialXpoBuilder]
    public class BasicXpoWithoutParentBuilder : BasicXpoParent
    {
        public BasicXpoWithoutParentBuilder(Session session)
            : base(session) { }

        public string OwnStringProperty { get; set;}
    }
}");


    [Fact]
    public Task BasicXpoWithParentBuilder()
        => RunSourceTest("BasicXpoWithoutParentBuilder.cs",
    @"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpo : BasicXpoParent
    {
        public BasicXpo(Session session)
            : base(session) { }

        public string OwnStringProperty { get; set;}
    }

    [XenialXpoBuilder]
    public class BasicXpoParent : XPObject
    {
        public BasicXpoParent(Session session)
            : base(session) { }

        public string ParentStringProperty { get; set;}
    }
}");


    [Fact]
    public Task BasicXpoWithParentAndGrandParentBuilder()
        => RunSourceTest("BasicXpoWithParentAndGrandParentBuilder.cs",
    @"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    [XenialXpoBuilder]
    public class BasicXpo : BasicXpoParent
    {
        public BasicXpo(Session session)
            : base(session) { }

        public string OwnStringProperty { get; set;}
    }

    [XenialXpoBuilder]
    public class BasicXpoParent : BasicXpoGrandParent
    {
        public BasicXpoParent(Session session)
            : base(session) { }

        public string ParentStringProperty { get; set;}
    }

    [XenialXpoBuilder]
    public class BasicXpoGrandParent : XPObject
    {
        public BasicXpoGrandParent(Session session)
            : base(session) { }

        public string GrantParentStringProperty { get; set;}
    }
}");


    [Fact]
    public Task BasicXpoWithReference()
        => RunSourceTest("BasicXpoWithReference.cs",
    @"using Xenial;
using DevExpress.Xpo;

namespace MyProject
{
    public class ReferenceXpo : XPObject
    {
        public ReferenceXpo(Session session)
            : base(session) { }

        public string ReferenceStringProperty { get; set;}
    }

    [XenialXpoBuilder]
    public class XpoObject : XPObject
    {
        public XpoObject(Session session)
            : base(session) { }

        public string StringProperty { get; set;}

        public ReferenceXpo ReferenceProperty { get; set;}
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
