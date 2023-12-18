using DevExpress.ExpressApp.Win.SystemModule;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout label item. </summary>
[XenialCheckLicense]
[XenialModelOptions(typeof(IModelLabel))]
public partial record LayoutLabelItem : LayoutViewItemNode
{
    /// <summary>   Gets the text. </summary>
    ///
    /// <value> The name of the image. </value>

    public string Text { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutStaticImageItem"/> class.
    /// </summary>
    ///
    /// <param name="text"> The text. </param>

    public LayoutLabelItem(string text)
        => (Text, Id) = (text, Slugifier.GenerateSlug(text));

    /// <summary>   Initializes a new instance of the <see cref="LayoutLabelItem"/> class. </summary>
    ///
    /// <param name="id">   The identifier. </param>
    /// <param name="text"> The text. </param>

    public LayoutLabelItem(string id, string text)
        => (Text, Id) = (text, Slugifier.GenerateSlug(id));
}
