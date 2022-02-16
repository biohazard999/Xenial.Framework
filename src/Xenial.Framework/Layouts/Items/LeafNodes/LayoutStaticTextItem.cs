using System;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout static text item. </summary>
[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelStaticText), IgnoredMembers = new[]
    {
        nameof(IModelStaticText.Id),
        nameof(IModelStaticText.Index),
        nameof(IModelStaticText.Text),
        nameof(IModelViewItem.Caption)
    }
)]
public partial record LayoutStaticTextItem : LayoutViewItem
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "By Convention")]
    internal bool WasTextSet => true;
    /// <summary>   Gets the text. </summary>
    ///
    /// <value> The name of the image. </value>
    [XenialAutoMapped]
    public string Text { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutStaticTextItem"/> class.
    /// </summary>
    ///
    /// <param name="text"> The text. </param>
    public LayoutStaticTextItem(string text) : this(text, text) { }

    /// <summary>
    /// 
    /// </summary>
    /// 
    /// <param name="text"></param>
    /// <param name="id"></param>
    public LayoutStaticTextItem(string text, string id) : base(id)
        => Text = text;
}

