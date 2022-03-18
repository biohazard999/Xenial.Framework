using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Xenial.Framework.Generators.Tests;

using Xunit;
using Xenial.Cli.Engine.Syntax;
using VerifyXunit;
using Microsoft.CodeAnalysis.Formatting;

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
