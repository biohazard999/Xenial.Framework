using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

using Shouldly;

using VerifyXunit;

using Xenial.Cli.Engine.Syntax;
using Xenial.Framework.Generators.Tests;

using Xunit;

namespace Xenial.Cli.Tests.Engine.Syntax;

public class MyModule : DevExpress.ExpressApp.ModuleBase
{
}

[UsesVerify]
public class InjectXenialLayoutBuilderModuleSyntaxRewriterTests
{
    [Fact]
    public async Task ShouldIgnoreEmptyCode()
    {
        var root = await RewriteCode("");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    [Fact]
    public async Task AddsLayoutBuilderMethod()
    {
        var root = await RewriteCode(@"public class MyModule : DevExpress.ExpressApp.ModuleBase
{
}");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }


    private static async Task<SyntaxNode> RewriteCode(string classCode)
    {
        var references = TestReferenceAssemblies.DefaultReferenceAssemblies
            .Concat(new[]
            {
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

        var rewriter = new InjectXenialLayoutBuilderModuleSyntaxRewriter(compilation);

        root = rewriter.Visit(root)!;

        using var ws = new AdhocWorkspace();

        return Formatter.Format(root, ws);
    }

}
