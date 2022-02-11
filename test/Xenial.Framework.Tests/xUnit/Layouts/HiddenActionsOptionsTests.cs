using System;
using System.Linq;
using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Tests.Assertions;

using Xunit;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;

namespace Xenial.Framework.Tests.xUnit.Layouts;

[UsesVerify]
public class HiddenActionsOptionsTests
{
    [Fact]
    public async Task TestHiddenActions()
    {
        var detailView = CreateComplexDetailViewWithLayout<LayoutViewItemBusinessObject>(
            _ => LayoutViewItemBusinessObjectLayoutBuilderWithHiddenActions.BuildLayout());

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }
}

public partial class LayoutViewItemBusinessObjectLayoutBuilderWithHiddenActions
    : LayoutBuilder<LayoutViewItemBusinessObject>
{
    public static Layout BuildLayout() => new(new()
    {
        Extensions = x =>
        {
            x.HiddenActions(new()
            {
                "Save",
                "SaveAndNew"
            });
        }
    })
    {
        Editor.MyString
    };
}
