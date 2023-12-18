using System;
using System.Linq;

#pragma warning disable CA1724 //Conflicts with Winforms: Should not conflict in practice
#pragma warning disable CA2227 //Collection fields should not have a setter: By Design

namespace Xenial.Framework.Layouts.Items.Base;

/// <summary>   (Immutable) a layout. </summary>
[XenialCheckLicense]
public partial record Layout : LayoutItem
{
    /// <summary>
    /// 
    /// </summary>
    public Layout() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public Layout(DetailViewOptions options)
        => Options = options ?? new();

    /// <summary>
    /// Specifies the <see cref="Xenial.Framework.Layouts.DetailViewOptions"></see> to set the options for the DetailView
    /// This only will work on top level View nodes, not included ones.
    /// </summary>
    public DetailViewOptions Options { get; set; } = new();
}
