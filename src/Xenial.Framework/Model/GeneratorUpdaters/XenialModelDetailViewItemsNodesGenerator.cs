
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

using static Xenial.Framework.Model.GeneratorUpdaters.NodeVisitors;

namespace Xenial.Framework.Model.GeneratorUpdaters;


/// <summary>
/// 
/// </summary>
/// <param name="items"></param>
/// <param name="layoutItemLeaf"></param>
/// <returns></returns>
public delegate IModelViewItem CreateViewItem(IModelViewItems items, LayoutViewItem layoutItemLeaf);

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="items"></param>
/// <param name="layoutItemLeaf"></param>
/// <returns></returns>
public delegate IModelViewItem CreateViewItem<T>(IModelViewItems items, T layoutItemLeaf)
    where T : LayoutViewItem;

/// <summary>
/// 
/// </summary>
public sealed class ViewItemNodeFactory
{
    internal Dictionary<Type, CreateViewItem> ViewItemFactories { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TViewItem"></typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    public ViewItemNodeFactory Register<TViewItem>(CreateViewItem<TViewItem> factory)
        where TViewItem : LayoutViewItem
    {
        ViewItemFactories[typeof(TViewItem)] = (items, item) => factory(items, (TViewItem)item);
        return this;
    }

    private CreateViewItem? FindFactory(Type? type)
    {
        if (type is null)
        {
            return null;
        }
        if (ViewItemFactories.TryGetValue(type, out var modelViewLayoutElementFactory))
        {
            return modelViewLayoutElementFactory;
        }
        return FindFactory(type.GetBaseType());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="layoutItemNode"></param>
    /// <returns></returns>
    public IModelViewItem? CreateViewItem(IModelViewItems parentNode, LayoutViewItem layoutItemNode)
    {
        _ = parentNode ?? throw new ArgumentNullException(nameof(parentNode));
        _ = layoutItemNode ?? throw new ArgumentNullException(nameof(layoutItemNode));

        var builder = FindFactory(layoutItemNode.GetType());

        if (builder is not null)
        {
            return builder(parentNode, layoutItemNode);
        }

        return null;
    }
}

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
public sealed partial class XenialModelDetailViewItemsNodesGenerator : ModelNodesGeneratorUpdater<ModelDetailViewItemsNodesGenerator>
{
    private static MemberEditorInfoCalculator MemberEditorInfoCalculator { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    public static ViewItemNodeFactory NodeFactory { get; } = new ViewItemNodeFactory()
            .Register<LayoutPropertyEditorItem>((viewItems, layoutItemNode) =>
            {
                var viewItem = viewItems.AddNode<IModelPropertyEditor>(layoutItemNode.Id);
                viewItem.PropertyName = layoutItemNode.ViewItemId;

                //For whatever reason we need to reset the editor here
                viewItem.ClearValue(nameof(viewItem.PropertyEditorType));

                if (!string.IsNullOrEmpty(layoutItemNode.EditorAlias))
                {
                    viewItem.PropertyEditorType
                        = MemberEditorInfoCalculator.GetEditorType(
                            viewItem.ModelMember,
                            layoutItemNode.EditorAlias
                    );
                }

                return viewItem;
            })
            .Register<LayoutStaticTextItem>((viewItems, layoutItemNode) =>
            {
                var newViewItem = viewItems.AddNode<IModelStaticText>(layoutItemNode.Id);

                newViewItem.Text = layoutItemNode.Text;

                return newViewItem;
            })
            .Register<LayoutStaticImageItem>((viewItems, layoutItemNode) =>
            {
                var newViewItem = viewItems.AddNode<IModelStaticImage>(layoutItemNode.Id);

                newViewItem.ImageName = layoutItemNode.ImageName;

                return newViewItem;
            })
            .Register<LayoutActionContainerItem>((viewItems, layoutItemNode) =>
            {
                var newViewItem = viewItems.AddNode<IModelActionContainerViewItem>(layoutItemNode.Id);

                if (viewItems.Application.ActionDesign is IModelActionToContainerMapping modelActionToContainerMapping)
                {
                    newViewItem.ActionContainer = modelActionToContainerMapping[layoutItemNode.ActionContainerId];
                }

                return newViewItem;
            })
            .Register<LayoutDashboardViewItem>((viewItems, layoutItemNode) =>
            {
                var newViewItem = viewItems.AddNode<IModelDashboardViewItem>(layoutItemNode.Id);

                newViewItem.View = viewItems.Application.Views.FirstOrDefault(m => m.Id == layoutItemNode.DashboardViewId);

                return newViewItem;
            });

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelViewItems viewItems)
        {
            if (viewItems.Parent is IModelDetailView modelDetailView)
            {
                var builder = XenialDetailViewLayoutNodesGeneratorUpdater.FindFunctor(modelDetailView);
                if (builder is null)
                {
                    return;
                }

                var layout = XenialDetailViewLayoutNodesGeneratorUpdater.InvokeBuilder(builder, modelDetailView);

                if (layout.Options is not null)
                {
                    ViewOptionsMapper.Map(layout.Options, modelDetailView);
                }

                XenialDetailViewLayoutNodesGeneratorUpdater.MarkDuplicateNodes(layout);

                foreach (var layoutViewItemNode in VisitNodes<LayoutViewItem>(layout))
                {
                    var viewItem = viewItems.OfType<IModelViewItem>().FirstOrDefault(m =>
                        m.Id == (layoutViewItemNode.IsDuplicate
                        ? layoutViewItemNode.Id
                        : layoutViewItemNode.ViewItemId)
                    );

                    if (viewItem is null)
                    {
                        var newViewItem = NodeFactory.CreateViewItem(viewItems, layoutViewItemNode);

                        viewItem = newViewItem;
                    }
                }

                foreach (var layoutViewItemNode in VisitNodes<LayoutViewItem>(layout))
                {
                    var viewItem = viewItems.OfType<IModelViewItem>().FirstOrDefault(m =>
                        m.Id == (layoutViewItemNode.IsDuplicate
                        ? layoutViewItemNode.Id
                        : layoutViewItemNode.ViewItemId)
                    );

                    ViewItemMapper.Map((LayoutItemNode)layoutViewItemNode, viewItem);
                }
            }
        }
    }
}
