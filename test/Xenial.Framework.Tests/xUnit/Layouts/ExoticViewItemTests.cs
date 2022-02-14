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
public class ExoticViewItemTests
{
    [Fact]
    public async Task TestLayoutViewItem()
    {
        var detailView = CreateComplexDetailViewWithLayout<ExoticViewItemBusinessObject>(_ => new ExoticViewItemBusinessObjectLayoutBuilder().BuildLayout());

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }
}

public class ExoticViewItemBusinessObject
{
    public string MyString { get; set; }
}

public partial class ExoticViewItemBusinessObjectLayoutBuilder : LayoutBuilder<ExoticViewItemBusinessObject>
{
    public Layout BuildLayout() => new()
    {
        Editor.MyString,
        Splitter(),
        Editor.MyString with
        {
            Caption = "My Caption 2"
        },
        Separator(),
        Editor.MyString with
        {
            Caption = "My Caption 3"
        }
    };
}
