using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using Xenial.Framework.Tests.Assertions;

using VerifyXunit;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using Xenial.Framework.Layouts;
using Xenial.Framework.Tests.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;

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

    [Fact]
    public async Task LayoutVerificationSimpleXml()
    {
        var detailView = CreateComplexDetailViewWithLayout(_ => new LayoutBuilderSimpleBusinessObject().BuildLayout());

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }

    [Fact]
    public async Task LayoutVerificationSimpleHtml()
    {
        var detailView = CreateComplexDetailViewWithLayout(_ => new LayoutBuilderSimpleBusinessObject().BuildLayout());

        var (html, _) = detailView.VisualizeModelNode();

        await Verifier.Verify(html).UseExtension("html");
    }

    [Fact]
    public async Task NestedLayoutVerificationSimpleXml()
    {
        var detailView = CreateNestedDetailViewWithLayout();

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }

    [Fact]
    public async Task NestedLayoutDuplicatedVerificationSimpleXml()
    {
        var detailView = CreateNestedDetailViewWithLayout(l => new()
        {
            l.PropertyEditor(m => m.OwnString),
            l.PropertyEditor(m => m.OwnString) with { Caption = "Second String" },
            l.PropertyEditor(m => m.NestedObject.BoolProperty),
            l.PropertyEditor(m => m.NestedObject.BoolProperty) with { Caption = "Second Bool" }
        });

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }
}

public partial class LayoutBuilderSimpleBusinessObject : LayoutBuilder<SimpleBusinessObject>
{
    public Layout BuildLayout() => new()
    {
        PropertyEditor(m => m.BoolProperty),
        PropertyEditor(m => m.IntegerProperty),
        PropertyEditor(m => m.NullableBoolProperty),
        PropertyEditor(m => m.NullableIntegerProperty),
        PropertyEditor(m => m.ObjectProperty),
        PropertyEditor(m => m.StringProperty)
    };
}
