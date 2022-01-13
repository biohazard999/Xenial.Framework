
using DevExpress.ExpressApp.Editors;

namespace Xenial.Framework.Layouts.Items.PubTernal;

/// <summary>   Interface for layout item node with align. </summary>
public interface ILayoutItemNodeWithAlign
{
    /// <summary>   Gets or sets the horizontal align. </summary>
    ///
    /// <value> The horizontal align. </value>

    StaticHorizontalAlign? HorizontalAlign { get; set; }

    /// <summary>   Gets or sets the vertical align. </summary>
    ///
    /// <value> The vertical align. </value>

    StaticVerticalAlign? VerticalAlign { get; set; }
}
