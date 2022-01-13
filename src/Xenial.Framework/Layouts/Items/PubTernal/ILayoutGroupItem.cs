using DevExpress.ExpressApp.Layout;

namespace Xenial.Framework.Layouts.Items.PubTernal;

/// <summary>   Interface ILayoutGroupItem. </summary>
public interface ILayoutGroupItem
{
    /// <summary>   Gets the direction. </summary>
    ///
    /// <value> The direction. </value>

    public FlowDirection Direction { get; init; }

    /// <summary>   Gets or sets the name of the image. </summary>
    ///
    /// <value> The name of the image. </value>

    public string? ImageName { get; set; }

    /// <summary>   Gets or sets the is collapsible group. </summary>
    ///
    /// <value> The is collapsible group. </value>

    public bool? IsCollapsibleGroup { get; set; }
}
