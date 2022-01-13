
using DevExpress.ExpressApp.Editors;

using Xenial.Framework.Layouts.Items.PubTernal;

namespace Xenial.Framework.Layouts.Items.Base;

/// <summary>   (Immutable) a layout item node with align. </summary>
[XenialCheckLicense]
public abstract partial record LayoutItemNodeWithAlign : LayoutItemNode, ILayoutItemNodeWithAlign
{
    /// <summary>   Gets or sets the horizontal align. </summary>
    ///
    /// <value> The horizontal align. </value>

    public StaticHorizontalAlign? HorizontalAlign { get; set; }

    /// <summary>   Gets or sets the vertical align. </summary>
    ///
    /// <value> The vertical align. </value>

    public StaticVerticalAlign? VerticalAlign { get; set; }
}
