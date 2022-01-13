using System;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout static text item. </summary>
[XenialCheckLicense]
public partial record LayoutStaticTextItem : LayoutViewItem
{
    /// <summary>   Gets the text. </summary>
    ///
    /// <value> The name of the image. </value>

    public string Text { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutStaticTextItem"/> class.
    /// </summary>
    ///
    /// <param name="text"> The text. </param>

    public LayoutStaticTextItem(string text)
        : base(text)
            => Text = text;

    /// <summary>   Gets or sets the text options. </summary>
    ///
    /// <value> The text options. </value>

    public Action<IModelStaticText>? TextOptions { get; set; }
}
