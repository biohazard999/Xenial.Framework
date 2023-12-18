using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout static text item. </summary>
[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelSeparatorBase), IgnoredMembers = new[]
    {
        nameof(IModelSeparatorBase.Id),
    }
)]
public partial record LayoutSeparatorItem : LayoutItemLeaf
{
    /// <summary>   Creates this instance. </summary>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

    public static LayoutSeparatorItem Create()
        => new();

    /// <summary>   Creates the specified identifier. </summary>
    ///
    /// <param name="id">   The identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

    public static LayoutSeparatorItem Create(string id)
        => new(id);

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutSeparatorItem"/> class.
    /// </summary>

    public LayoutSeparatorItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutSeparatorItem"/> class.
    /// </summary>
    ///
    /// <param name="id">   The identifier. </param>

    public LayoutSeparatorItem(string id)
        => Id = Slugifier.GenerateSlug(id);
}

