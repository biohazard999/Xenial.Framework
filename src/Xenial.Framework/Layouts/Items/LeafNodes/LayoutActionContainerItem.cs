#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1000 // Don't declare static members in generic types
#pragma warning disable IDE1006 // Naming Styles

using System;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout action container item. </summary>
[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelActionContainerViewItem), IgnoredMembers = new[]
    {
        nameof(IModelActionContainerViewItem.Id),
        nameof(IModelActionContainerViewItem.Index),
        nameof(IModelActionContainerViewItem.ActionContainer)
    }
)]
public partial record LayoutActionContainerItem : LayoutViewItem
{
    /// <summary>
    /// 
    /// </summary>
    public string ActionContainerId { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionContainerId"></param>
    public LayoutActionContainerItem(string actionContainerId!!) : this(actionContainerId, actionContainerId) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionContainerId"></param>
    /// <param name="id"></param>
    public LayoutActionContainerItem(string actionContainerId!!, string id) : base(id)
        => ActionContainerId = actionContainerId;
}
