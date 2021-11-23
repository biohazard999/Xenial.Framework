﻿using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout empty space item. </summary>
[XenialCheckLicense]
public partial record LayoutEmptySpaceItem : LayoutItemLeaf
{
    /// <summary>   Creates this instance. </summary>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

    public static LayoutEmptySpaceItem Create()
        => new();

    /// <summary>   Creates the specified identifier. </summary>
    ///
    /// <param name="id">   The identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

    public static LayoutEmptySpaceItem Create(string id)
        => new(id);

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutEmptySpaceItem"/> class.
    /// </summary>

    public LayoutEmptySpaceItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutEmptySpaceItem"/> class.
    /// </summary>
    ///
    /// <param name="id">   The identifier. </param>

    public LayoutEmptySpaceItem(string id)
        => Id = Slugifier.GenerateSlug(id);
}
