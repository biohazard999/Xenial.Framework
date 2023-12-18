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
public class LayoutGenericOptionsTests
{
    [Fact]
    public async Task TestGenericOptions()
    {
        var detailView = CreateComplexDetailViewWithLayout<LayoutViewItemBusinessObject>(
            _ => LayoutViewItemBusinessObjectLayoutBuilderWithOptions.BuildLayout());

        var (_, xml) = detailView.VisualizeModelNode();

        await Verifier.Verify(xml).UseExtension("xml");
    }
}

public partial class LayoutViewItemBusinessObjectLayoutBuilderWithOptions
    : LayoutBuilder<LayoutViewItemBusinessObject>
{
    public static Layout BuildLayout() => new(new()
    {
        Extensions = x =>
        {
            x.Generic(new()
            {
                ["AllowEdit"] = false
            });
        }
    })
    {
        Editor.MyString
    };
}
