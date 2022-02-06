using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Tests.Assertions;

using Xunit;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;

namespace Xenial.Framework.Tests.xUnit.Layouts;

[UsesVerify]
public class LayoutViewItemTests
{
    [Fact]
    public async Task TestLayoutViewItem()
    {
        var detailView = CreateComplexDetailViewWithLayout<LayoutViewItemBusinessObject>(_ => new LayoutViewItemBusinessObjectLayoutBuilder().BuildLayout());

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }
}

public class LayoutViewItemBusinessObject
{
    //public string MyString { get; set; }
}

public partial class LayoutViewItemBusinessObjectLayoutBuilder : LayoutBuilder<LayoutViewItemBusinessObject>
{
    public Layout BuildLayout() => new()
    {
        //Editor.MyString,
        //Editor.MyString with
        //{
        //    Caption = "My Caption 2"
        //},
        StaticTextItem("This is a text"),
        //StaticImageItem("BO_Customer")
    };
}
