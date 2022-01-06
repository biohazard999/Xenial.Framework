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
    /// Specifies the <see cref="Xenial.Framework.Layouts.DetailViewOptions"></see> to set the options for the DetailView
    /// This only will work on top level View nodes, not included ones.
    /// </summary>
    public DetailViewOptions Options { get; set; } = new();
}

/// <summary>
/// 
/// </summary>
public static class LayoutExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Layout WithOptions(this Layout layout, Func<DetailViewOptions, DetailViewOptions> options)
    {
        _ = layout ?? throw new ArgumentNullException(nameof(layout));
        _ = options ?? throw new ArgumentNullException(nameof(options));

        layout.Options = options.Invoke(layout.Options);
        return layout;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="children"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Layout WithChildren(this Layout layout, params LayoutItemNode[] children)
    {
        _ = layout ?? throw new ArgumentNullException(nameof(layout));

        layout.Add(children);

        return layout;
    }
}
