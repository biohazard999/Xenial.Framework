using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.Utils;

using Locations = DevExpress.Persistent.Base.Locations;

namespace Xenial.Framework.Layouts.Items.PubTernal;

/// <summary>   Interface ILayoutElementWithCaptionOptions. </summary>
public interface ILayoutElementWithCaptionOptions
{
    /// <summary>   Gets or sets a value indicating whether [show caption]. </summary>
    ///
    /// <value>
    /// <c>null</c> if [show caption] contains no value, <c>true</c> if [show caption]; otherwise,
    /// <c>false</c>.
    /// </value>

    bool? ShowCaption { get; set; }

    /// <summary>   Gets or sets the caption location. </summary>
    ///
    /// <value> The caption location. </value>

    Locations? CaptionLocation { get; set; }

    /// <summary>   Gets or sets the caption horizontal alignment. </summary>
    ///
    /// <value> The caption horizontal alignment. </value>

    HorzAlignment? CaptionHorizontalAlignment { get; set; }

    /// <summary>   Gets or sets the caption vertical alignment. </summary>
    ///
    /// <value> The caption vertical alignment. </value>

    VertAlignment? CaptionVerticalAlignment { get; set; }

    /// <summary>   Gets or sets the caption word wrap. </summary>
    ///
    /// <value> The caption word wrap. </value>

    WordWrap? CaptionWordWrap { get; set; }
}
