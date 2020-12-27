#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1000 // Don't declare static members in generic types
#pragma warning disable IDE1006 // Naming Styles

using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items
{
    /// <summary>
    /// 
    /// </summary>
    [XenialCheckLicence]
    public partial record LayoutActionContainerItem(string ActionContainerId) : LayoutViewItem(ActionContainerId)
    {
        /// <summary>
        /// Gets or sets the paint style.
        /// </summary>
        /// <value>The paint style.</value>
        public ActionItemPaintStyle? PaintStyle { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public ActionContainerOrientation? Orientation { get; set; }

        /// <summary>
        /// Gets or sets the action container options.
        /// </summary>
        /// <value>The action container options.</value>
        public Action<IModelActionContainerViewItem>? ActionContainerOptions { get; set; }

    }
}
