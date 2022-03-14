using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Framework.DevTools.X2C;

using Xunit;

namespace Xenial.Framework.DevTools.Tests.X2C;

[UsesVerify]
public class X2CEngineSanityTests
{
    private X2CEngine X2C { get; } = new();

    internal const string ListViewXml = @"<ListView Id=""FooBar_ListView""
          ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBar"">
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
        var code = X2C.ConvertToCode(ListViewXml);

        await Verifier.Verify(code).UseExtension("cs");
    }
}
