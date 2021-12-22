using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using Xenial.Framework.Tests.Assertions;

using VerifyXunit;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;

namespace Xenial.Framework.Tests.xUnit;

[VerifyXunit.UsesVerify]
public class VerificationTests
{
    static VerificationTests() => VerifyTests.VerifyAngleSharpDiffing.Initialize();

    [Fact]
    public async Task LayoutVerificationXml()
    {
        var detailView = CreateDetailView();

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }

    [Fact]
    public async Task LayoutVerificationHtml()
    {
        var detailView = CreateDetailView();

        var (html, _) = detailView.VisualizeModelNode();

        await Verifier.Verify(html).UseExtension("html");
    }

    private static DevExpress.ExpressApp.Model.IModelDetailView? CreateDetailView() => CreateComplexDetailViewWithLayout(l => new()
    {
        l.TabbedGroup() with
        {
            Children = new()
            {
                l.Tab("FirstTab") with
                {
                    ImageName = "SomeImage"
                }
            }
        }
    });
}
