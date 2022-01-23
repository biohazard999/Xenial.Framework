#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1000 // Don't declare static members in generic types
#pragma warning disable IDE1006 // Naming Styles

using System;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout dashboard view item. </summary>
[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelDashboardViewItem), IgnoredMembers = new[]
    {
        nameof(IModelDashboardViewItem.Id),
        nameof(IModelDashboardViewItem.Index),
        nameof(IModelDashboardViewItem.View),
        nameof(IModelViewItem.Caption),
    }
)]
public partial record LayoutDashboardViewItem(string DashboardViewId)
    : LayoutViewItem(DashboardViewId)
{
}
