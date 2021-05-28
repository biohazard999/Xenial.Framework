using ToolTipIconType = DevExpress.Persistent.Base.ToolTipIconType;

namespace Xenial.Framework.Layouts.Items.PubTernal
{
    /// <summary>   Interface ILayoutToolTipOptions. </summary>
    public interface ILayoutToolTipOptions
    {
        /// <summary>   Gets or sets the type of the tool tip icon. </summary>
        ///
        /// <value> The type of the tool tip icon. </value>

        ToolTipIconType? ToolTipIconType { get; set; }

        /// <summary>   Gets or sets the tool tip title. </summary>
        ///
        /// <value> The tool tip title. </value>

        string? ToolTipTitle { get; set; }
    }
}
