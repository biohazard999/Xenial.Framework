using System;

using DevExpress.ExpressApp.Model;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles

namespace Xenial.Framework.Layouts.Items.Base;

[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelMemberViewItem), IgnoredMembers = new[]
    {
        nameof(IModelMemberViewItem.Id),
        nameof(IModelMemberViewItem.Index),
        nameof(IModelMemberViewItem.Caption)
    }
)]
public partial record LayoutMemberViewItem : LayoutViewItem
{
    public LayoutMemberViewItem(string viewItemId) : base(viewItemId)
    {
    }

    protected LayoutMemberViewItem(LayoutViewItem original) : base(original)
    {
    }
}
