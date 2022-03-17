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

namespace Xenial.Cli.Tests.Engine.Syntax;

[UsesVerify]
public class LayoutBuilderAttributeSyntaxRewriterTests
{
    [Fact]
    public async Task EmptyCode()
    {
        var root = await RewriteCode(@"");
        await Verifier.Verify(root.ToFullString()).UseExtension("cs");
    }

    private static async Task<SyntaxNode> RewriteCode(string classCode)
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
        var rewriter = new LayoutBuilderAttributeSyntaxRewriter(semanticModel);

        return rewriter.Visit(root)!;
    }

}
