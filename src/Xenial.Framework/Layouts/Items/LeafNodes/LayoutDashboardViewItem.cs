#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1000 // Don't declare static members in generic types
#pragma warning disable IDE1006 // Naming Styles

using System;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes
{
    /// <summary>   (Immutable) a layout dashboard view item. </summary>
    [XenialCheckLicense]
    public partial record LayoutDashboardViewItem(string DashboardViewId) : LayoutViewItem(DashboardViewId)
    {
        /// <summary>   Gets or sets the criteria operator. </summary>
        ///
        /// <value> The criteria operator. </value>

        public string? Criteria { get; set; }

        /// <summary>   Gets or sets the actions toolbar visibility. </summary>
        ///
        /// <value> The actions toolbar visibility. </value>

        public ActionsToolbarVisibility? ActionsToolbarVisibility { get; set; }

        /// <summary>   Gets or sets the dashboard options. </summary>
        ///
        /// <value> The dashboard options. </value>

        public Action<IModelDashboardViewItem>? DashboardViewOptions { get; set; }
    }
}
