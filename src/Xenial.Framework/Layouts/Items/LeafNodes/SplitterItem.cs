using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout static text item. </summary>
[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelSplitterBase), IgnoredMembers = new[]
    {
        nameof(IModelSplitterBase.Id),
    }
)]
public partial record LayoutSplitterItem : LayoutItemLeaf
{
    /// <summary>   Creates this instance. </summary>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

    public static LayoutSplitterItem Create()
        => new();

    /// <summary>   Creates the specified identifier. </summary>
    ///
    /// <param name="id">   The identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

    public static LayoutSplitterItem Create(string id)
        => new(id);

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutSplitterItem"/> class.
    /// </summary>

    public LayoutSplitterItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutSplitterItem"/> class.
    /// </summary>
    ///
    /// <param name="id">   The identifier. </param>

    public LayoutSplitterItem(string id)
        => Id = Slugifier.GenerateSlug(id);
}

