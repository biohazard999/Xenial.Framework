using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks.Generators;

/// <summary>
/// 
/// </summary>
[XenialModelOptions(typeof(IModelJumplists), IgnoredMembers = new[]
{
    nameof(IModelJumplists.Index),
})]
public partial record ModelJumplistOptions
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

/// <summary>
/// 
/// </summary>
[XenialModelOptionsMapper(typeof(ModelJumplistItemBase))]
[XenialModelOptionsMapper(typeof(ModelJumplistActionItem))]
[XenialModelOptionsMapper(typeof(ModelJumplistLaunchItem))]
[XenialModelOptionsMapper(typeof(ModelJumplistProtocolItem))]
[XenialModelOptionsMapper(typeof(ModelJumplistSeperatorItem))]
[XenialModelOptionsMapper(typeof(ModelJumplistNavigationItem))]
[XenialModelOptionsMapper(typeof(ModelJumplistViewItem))]
[XenialModelOptionsRootMapper(typeof(ModelJumplistItem))]
public sealed partial class ModelJumplistTasksCategoryGeneratorUpdater : ModelNodesGeneratorUpdater<ModelJumplistTasksCategoryGenerator>
{
    private readonly ModelJumplistOptions options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ModelJumplistTasksCategoryGeneratorUpdater(ModelJumplistOptions options!!)
        => this.options = options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node!!)
    {
        if (node is IModelJumplistTaskCategory taskCategoryNode)
        {
            foreach (var item in options.TaskCategory)
            {
                FactorNode(item, taskCategoryNode);
            }
        }
    }

    static partial void MapNodeCore(ModelJumplistItemBase from, IModelJumplistItemBase to)
    {
        if (!string.IsNullOrEmpty(from.ProtocolId))
        {
            if (to.Application.Options is IModelOptionsDeeplinkProtocols modelOptionsDeeplinkProtocols)
            {
                to.Protocol = modelOptionsDeeplinkProtocols.DeeplinkProtocols[from.ProtocolId];
            }
        }
    }


    static partial void MapNodeCore(ModelJumplistNavigationItem from, IModelJumplistItemNavigationItem to)
    {
        if (to.Application is IModelApplicationNavigationItems navigationItems)
        {
            to.NavigationItem = navigationItems
                .NavigationItems
                .AllItems
                .FirstOrDefault(m => m.Id == from.NavigationItemId);
        }
    }

    static partial void MapNodeCore(ModelJumplistViewItem from, IModelJumplistItemView to)
        => to.View = to.Application.Views[from.ViewId];

    static partial void MapNodeCore(ModelJumplistActionItem from, IModelJumplistItemAction to)
        => to.ActionId = from.ActionId;

    internal static void FactorNode(ModelJumplistItem item, IModelNode node)
        => Map(item, item switch
        {
            ModelJumplistActionItem actionItem => node.AddNode<IModelJumplistItemAction>(actionItem.ActionId),
            ModelJumplistLaunchItem => node.AddNode<IModelJumplistItemLaunch>(),
            ModelJumplistProtocolItem => node.AddNode<IModelJumplistItemLaunch>(),
            ModelJumplistSeperatorItem => node.AddNode<IModelJumplistItemSeperator>(),
            ModelJumplistNavigationItem navigationItem => node.AddNode<IModelJumplistItemNavigationItem>(navigationItem.NavigationItemId),
            ModelJumplistViewItem viewItem => node.AddNode<IModelJumplistItemView>(viewItem.ViewId),
            _ => throw new NotImplementedException($"No factory for {item}")
        });
}

/// <summary>
/// 
/// </summary>
[XenialModelOptionsMapper(typeof(ModelJumplistCustomCategory))]
public sealed partial class ModelJumplistCustomCategoriesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelJumplistCustomCategoriesGenerator>
{
    private readonly ModelJumplistOptions options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ModelJumplistCustomCategoriesGeneratorUpdater(ModelJumplistOptions options!!)
        => this.options = options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node!!)
    {
        if (node is IModelJumplistCustomCategories modelJumpListCollection)
        {
            foreach (var category in options.CustomCategories)
            {
                var modelCategory = modelJumpListCollection.AddNode<IModelJumplistCustomCategory>(category.Caption);
                modelCategory.Caption = category.Caption;
                Map(category, modelCategory);

                foreach (var item in category.Items)
                {
                    ModelJumplistTasksCategoryGeneratorUpdater.FactorNode(item, modelCategory);
                }
            }
        }
    }
}

/// <summary>
/// 
/// </summary>
[XenialModelOptionsMapper(typeof(ModelJumplistOptions))]
public sealed partial class ModelJumplistOptionsGeneratorUpdaters : ModelNodesGeneratorUpdater<ModelOptionsNodesGenerator>
{
    private readonly ModelJumplistOptions options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ModelJumplistOptionsGeneratorUpdaters(ModelJumplistOptions options!!)
        => this.options = options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelOptions modelOptions && modelOptions is IModelOptionsJumplists modelOptionsJumplists)
        {
            MapNode(options, modelOptionsJumplists.Jumplists);
        }
    }
}
