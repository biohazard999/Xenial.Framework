using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Shouldly;

using VerifyXunit;

using Xenial.Framework.DevTools.X2C;
using Xenial.Framework.Generators.Tests;

using Xunit;
using Xunit.Abstractions;

namespace Xenial.Framework.DevTools.Tests.X2C;

[UsesVerify]
public class X2CEngineSanityTests
{
    private readonly ITestOutputHelper output;

    public X2CEngineSanityTests(ITestOutputHelper output)
        => this.output = output;

    internal const string ClassCode = @"namespace HtmlDemoXAFApplication.Module.BusinessObjects
{
    public class FooBarPersistent
    {
        public int Oid { get; set; }
        public string Label { get; set; }
    }
}";


    internal const string ListViewXml = @"<ListView Id=""FooBar_ListView""
          ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"">
  <Columns>
    <ColumnInfo Id=""Oid""
                PropertyName=""Oid""
                Index=""-1"" />
    <ColumnInfo Id=""Label""
                PropertyName=""Label""
                Index=""0"" />
  </Columns>
</ListView>
";

    [Fact]
    public async Task ConvertBasicListView()
    {
        var code = X2CEngine.ConvertToCode(ListViewXml);

        await Verifier.Verify(code).UseExtension("cs");
    }

    [Fact]
    public void CompileBasicListView()
    {
        var code = X2CEngine.ConvertToCode(ListViewXml);
        CompileCode(code);
    }

    internal const string DetailViewXml = @"<DetailView Id=""FooBarPersistent_DetailView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"">
  <Items>
    <PropertyEditor Id=""Label""
                    PropertyName=""Label"" />
    <PropertyEditor Id=""Oid""
                    PropertyName=""Oid"" />
  </Items>
  <Layout>
    <LayoutGroup Id=""Main"">
      <LayoutItem Id=""Label""
                  ViewItem=""Label""
                  Index=""0"" />
    </LayoutGroup>
  </Layout>
</DetailView>
";

    [Fact]
    public async Task ConvertBasicDetailView()
    {
        var code = X2CEngine.ConvertToCode(DetailViewXml);

        await Verifier.Verify(code).UseExtension("cs");
    }

    [Fact]
    public void CompileBasicDetailView()
    {
        var code = X2CEngine.ConvertToCode(DetailViewXml);
        CompileCode(code);
    }

    private void CompileCode(string code)
    {
        var references = TestReferenceAssemblies.DefaultReferenceAssemblies
            .Concat(new[]
            {
                MetadataReference.CreateFromFile(typeof(PersistentAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DomainComponentAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Xenial.Framework.XenialModuleBase).Assembly.Location),
            });

        var trees = new[]
        {
            CSharpSyntaxTree.ParseText(ClassCode),
            CSharpSyntaxTree.ParseText(code)
        };

        var compilation = CSharpCompilation.Create("test.dll",
               trees,
               references,
               new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var generatorDriver = CSharpGeneratorDriver.Create(new Xenial.Framework.Generators.XenialGenerator());

        generatorDriver.RunGeneratorsAndUpdateCompilation(compilation, out var c, out var diagnostics);
        compilation = (CSharpCompilation)c;
        using var dllStream = new MemoryStream();
        var result = compilation.Emit(dllStream);

        if (result.Diagnostics.Length > 0)
        {
            output.WriteLine($"Compilation failed: {result.Diagnostics.Length}");
            output.WriteLine(new string('=', 10));
            foreach (var diagnostic in result.Diagnostics)
            {
                output.WriteLine(diagnostic.ToString());
            }
            output.WriteLine(new string('=', 10));
        }

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBe(true, "Compilation failed"),
            //Unused references
            () => result.Diagnostics.Where(m => m.WarningLevel == 0).Count().ShouldBe(0)
        );
    }
}
