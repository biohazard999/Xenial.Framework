using System;
using System.Drawing;

using DevExpress.ExpressApp.Layout;

namespace Xenial.Framework.Layouts.Items.Base
{
    /// <summary>   (Immutable) a layout item leaf. </summary>
    [XenialCheckLicence]
    public partial record LayoutItemLeaf : LayoutItemNodeWithAlign
    {
        /// <summary>   Gets or sets the maximum size. </summary>
        ///
        /// <value> The maximum size. </value>

        public Size? MaxSize { get; set; }

        /// <summary>   Gets or sets the minimum size. </summary>
        ///
        /// <value> The minimum size. </value>

        public Size? MinSize { get; set; }

        /// <summary>   Gets or sets the type of the size constraints. </summary>
        ///
        /// <value> The type of the size constraints. </value>

        public XafSizeConstraintsType? SizeConstraintsType { get; set; }
    }
}
