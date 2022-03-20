using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

using VerifyXunit;

using Xenial.Cli.Engine.Syntax;
using Xenial.Framework.DevTools.X2C;
using Xenial.Framework.Generators.Tests;

using Xunit;

namespace Xenial.Cli.Tests.Engine.Syntax;

[UsesVerify]
public class MergeClassesSyntaxRewriterTests
{
    internal const string Code = @"using System;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;
using Xenial.Framework.Layouts.Items.Base;

namespace HtmlDemoXAFApplication.Module.BusinessObjects
{
    public sealed partial class FooBarPersistentLayoutBuilder : LayoutBuilder<FooBarPersistent>
    {
        public Layout BuildLayout() => new Layout();
    }
}
";
    internal const string CodeToMerge = @"using System;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;
using Xenial.Framework.Layouts.Items.Base;

namespace HtmlDemoXAFApplication.Module.BusinessObjects
{
    public sealed partial class FooBarPersistentLayoutBuilder : LayoutBuilder<FooBarPersistent>
    {
        public Layout BuildCompactLayout() => new Layout()
        {
        };
    }
}
";

    [Fact]
    public async Task SimpleCodeMerge()
    {
        var root = await RewriteCode(Code, new(
            "HtmlDemoXAFApplication.Module.BusinessObjects",
            "FooBarPersistent",
            "HtmlDemoXAFApplication.Module.BusinessObjects",
            "FooBarPersistentLayoutBuilder",
            "BuildCompactLayout",
            "FooBarPersistent_Compact_DetailView",
            CodeToMerge,
            "")
        );

        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    private static async Task<SyntaxNode> RewriteCode(string classCode, X2CCodeResult codeResult)
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
        var rewriter = new MergeClassesSyntaxRewriter(semanticModel, codeResult);

        root = rewriter.Visit(root)!;

        using var ws = new AdhocWorkspace();

        return Formatter.Format(root, ws);
    }
}
