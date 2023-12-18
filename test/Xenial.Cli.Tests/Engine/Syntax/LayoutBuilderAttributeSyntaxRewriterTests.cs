using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

using System;
using System.Linq;

using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Cli.Engine.Syntax;
using Xenial.Framework.Generators.Tests;

using Xunit;

namespace Xenial.Cli.Tests.Engine.Syntax;

[UsesVerify]
public class LayoutBuilderAttributeSyntaxRewriterTests
{
    internal const string ClassWithoutAttribute = @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    public class Position : BaseObject
    {
    }
}
";
    internal const string ClassWithAttribute = @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder))]
    public class Position : BaseObject
    {
    }
}
";
    [Fact]
    public async Task EmptyCode()
    {
        var root = await RewriteCode(new(""), "");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttribute()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder"), ClassWithoutAttribute);
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttributeAndNamespace()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder"), @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    public class Position : BaseObject
    {
    }
}
");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task OnlyAddsAttributeWhenNotPresent()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder"), @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder))]
    public class Position : BaseObject
    {
    }
}
");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttributeWithMethodName()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder") { LayoutBuilderMethod = "BuildCompactLayout" }, ClassWithoutAttribute);
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttributeWithMethodNameAndViewId()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder") { LayoutBuilderMethod = "BuildCompactLayout", ViewId = "Position_Compact_DetailView" }, ClassWithoutAttribute);
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task OnlyAddsAttributeWhenNotPresentWithMethodName()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder") { LayoutBuilderMethod = "BuildCompactLayout" }, @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder), nameof(PositionLayoutBuilder.BuildCompactLayout))]
    public class Position : BaseObject
    {
    }
}
");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttributeWhenNotPresentWithMethodName()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder") { LayoutBuilderMethod = "BuildCompactLayout" }, ClassWithAttribute);
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttributeWhenNotPresentWithMethodNameAndViewId()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder") { LayoutBuilderMethod = "BuildCompactLayout", ViewId = "Position_Compact_DetailView" }, ClassWithAttribute);
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task OnlyAddsAttributeWhenNotPresentWithMethodNameAndViewId()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder") { LayoutBuilderMethod = "BuildCompactLayout", ViewId = "Position_Compact_DetailView" }, @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder), nameof(PositionLayoutBuilder.BuildCompactLayout), ViewId = ""Position_Compact_DetailView"")]
    public class Position : BaseObject
    {
    }
}
");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttributeWhenNotPresentWithMethodNameAndDifferentViewId()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder") { LayoutBuilderMethod = "BuildCompactLayout", ViewId = "Position_Compact_DetailView" }, @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder), nameof(PositionLayoutBuilder.BuildCompactLayout), ViewId = ""Position_Complex_DetailView"")]
    public class Position : BaseObject
    {
    }
}
");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsAttributeWhenDifferentViewId()
    {
        var root = await RewriteCode(new("PositionLayoutBuilder"), @"using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder), ViewId = ""Position_Complex_DetailView"")]
    public class Position : BaseObject
    {
    }
}
");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    private static async Task<SyntaxNode> RewriteCode(LayoutAttributeInfo builderInfo, string classCode)
    {
        var references = TestReferenceAssemblies.DefaultReferenceAssemblies
            .Concat(new[]
            {
                MetadataReference.CreateFromFile(typeof(PersistentAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DomainComponentAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Xenial.Framework.XenialModuleBase).Assembly.Location),
            });

        var code = CSharpSyntaxTree.ParseText(classCode);

        var root = await code.GetRootAsync();

        var compilation = CSharpCompilation.Create("test.dll",
               new[] { code },
               references,
               new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(code);
        var rewriter = new LayoutBuilderAttributeSyntaxRewriter(semanticModel, builderInfo);

        root = rewriter.Visit(root)!;

        using var ws = new AdhocWorkspace();

        return Formatter.Format(root, ws);
    }
}
