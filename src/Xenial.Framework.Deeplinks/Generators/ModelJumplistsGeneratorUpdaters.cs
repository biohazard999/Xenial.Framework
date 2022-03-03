using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks.Generators;

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelJumplists), IgnoredMembers = new[]
{
    nameof(IModelJumplists.Index),
})]
public partial record ModelJumplists
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public List<ModelJumplistItem> TaskCategory { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public List<ModelJumplistCustomCategory> CustomCategories { get; set; } = new();
}

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelJumplistCustomCategory), IgnoredMembers = new[]
{
    nameof(IModelJumplistCustomCategory.Index),
    nameof(IModelJumplistCustomCategory.Caption),
})]
public partial record ModelJumplistCustomCategory(string Caption!!)
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
    public List<ModelJumplistItem> Items { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void Add(ModelJumplistItem item!!) => Items.Add(item);
}

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelJumplistItem), IgnoredMembers = new[]
{
    nameof(IModelJumplistItem.Index),
})]
public abstract partial record ModelJumplistItem
{
}

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelJumplistItemBase), IgnoredMembers = new[]
{
    nameof(IModelJumplistItemBase.Index),
    nameof(IModelJumplistItemBase.Protocol),
})]
public abstract partial record ModelJumplistItemBase : ModelJumplistItem
{
    /// <summary>
    /// 
    /// </summary>
    public string? ProtocolId { get; set; }
}

/// <summary>
/// 
/// </summary>
/// <param name="ActionId"></param>
[XenialModelOptions(typeof(IModelJumplistItemAction), IgnoredMembers = new[]
{
    nameof(IModelJumplistItemAction.Index),
    nameof(IModelJumplistItemAction.ActionId),
    nameof(IModelJumplistItemAction.Action),
    nameof(IModelJumplistItemAction.Protocol),
})]
public partial record ModelJumplistActionItem(string ActionId!!) : ModelJumplistItemBase
{
}

/// <summary>
/// 
/// </summary>
/// <param name="ProcessPath"></param>
[XenialModelOptions(typeof(IModelJumplistItemLaunch), IgnoredMembers = new[]
{
    nameof(IModelJumplistItemLaunch.Index),
    nameof(IModelJumplistItemLaunch.ProcessPath),
    nameof(IModelJumplistItemLaunch.Protocol),
})]
public partial record ModelJumplistLaunchItem(string ProcessPath) : ModelJumplistItemBase
{
}

/// <summary>
/// 
/// </summary>
/// <param name="NavigationItemId"></param>
[XenialModelOptions(typeof(IModelJumplistItemNavigationItem), IgnoredMembers = new[]
{
    nameof(IModelJumplistItemNavigationItem.Index),
    nameof(IModelJumplistItemNavigationItem.NavigationItem),
    nameof(IModelJumplistItemNavigationItem.Protocol),
})]
public partial record ModelJumplistNavigationItem(string NavigationItemId!!) : ModelJumplistItemBase
{
}

/// <summary>
/// 
/// </summary>
/// <param name="Verb"></param>
[XenialModelOptions(typeof(IModelJumplistItemProtocol), IgnoredMembers = new[]
{
    nameof(IModelJumplistItemProtocol.Index),
    nameof(IModelJumplistItemProtocol.Verb),
    nameof(IModelJumplistItemProtocol.Protocol),
})]
public partial record ModelJumplistProtocolItem(string Verb!!) : ModelJumplistItemBase
{
}

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelJumplistItemSeperator), IgnoredMembers = new[]
{
    nameof(IModelJumplistItemSeperator.Index)
})]
public partial record ModelJumplistSeperatorItem : ModelJumplistItem
{
}

/// <summary>
/// 
/// </summary>
/// <param name="ViewId"></param>
[XenialModelOptions(typeof(IModelJumplistItemView), IgnoredMembers = new[]
{
    nameof(IModelJumplistItemView.Index),
    nameof(IModelJumplistItemView.View),
    nameof(IModelJumplistItemView.Protocol),
})]
public partial record ModelJumplistViewItem(string ViewId!!) : ModelJumplistItemBase
{
}

/// <summary>
/// 
/// </summary>
public sealed class ModelJumplistTasksCategoryGenerator : ModelNodesGeneratorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    protected override void GenerateNodesCore(ModelNode node) { }
}

/// <summary>
/// 
/// </summary>
public sealed class ModelJumplistCustomCategoriesGenerator : ModelNodesGeneratorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    protected override void GenerateNodesCore(ModelNode node) { }
}


