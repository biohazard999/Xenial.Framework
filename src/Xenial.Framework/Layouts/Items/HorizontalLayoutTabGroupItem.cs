
using DevExpress.ExpressApp.Layout;

#pragma warning disable CA1710 //Rename Type to end in Collection -> By Design
#pragma warning disable CA2227 //Collection fields should not have a setter: By Design

namespace Xenial.Framework.Layouts.Items;

/// <summary>   (Immutable) a horizontal layout tab group item. </summary>
[XenialCheckLicense]
public partial record HorizontalLayoutTabGroupItem : LayoutTabGroupItem
{
    /// <summary>   Creates this instance. </summary>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.HorizontalLayoutTabGroupItem. </returns>

    public static new HorizontalLayoutTabGroupItem Create()
        => new();

    /// <summary>   Creates the specified identifier. </summary>
    ///
    /// <param name="id">   The identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.HorizontalLayoutTabGroupItem. </returns>

    public static new HorizontalLayoutTabGroupItem Create(string id)
        => new(id);

    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalLayoutTabGroupItem"/> class.
    /// </summary>

    public HorizontalLayoutTabGroupItem()
        => Direction = FlowDirection.Horizontal;

    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalLayoutTabGroupItem"/> class.
    /// </summary>
    ///
    /// <param name="id">   The identifier. </param>

    public HorizontalLayoutTabGroupItem(string id)
        : base(id, FlowDirection.Horizontal) { }
}
