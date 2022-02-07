using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

using static Xenial.Framework.Model.GeneratorUpdaters.NodeVisitors;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// 
/// </summary>
/// <param name="parent"></param>
/// <param name="layoutItemNode"></param>
/// <returns></returns>
public delegate IModelViewLayoutElement CreateViewLayoutElement(IModelNode parent, LayoutItemNode layoutItemNode);

/// <summary>
/// 
/// </summary>
/// <typeparam name="TLayoutElement"></typeparam>
/// <param name="parent"></param>
/// <param name="layoutItemNode"></param>
/// <returns></returns>
public delegate IModelViewLayoutElement CreateViewLayoutElement<TLayoutElement>(IModelNode parent, TLayoutElement layoutItemNode)
    where TLayoutElement : LayoutItemNode;

/// <summary>
/// 
/// </summary>
/// <param name="layoutItemNode"></param>
/// <param name="index"></param>
/// <returns></returns>
public delegate string CreateViewLayoutElementAutoId(LayoutItemNode layoutItemNode, int index);

/// <summary>
/// 
/// </summary>
/// <typeparam name="TLayoutElement"></typeparam>
/// <param name="layoutItemNode"></param>
/// <param name="index"></param>
/// <returns></returns>
public delegate string CreateViewLayoutElementAutoId<TLayoutElement>(TLayoutElement layoutItemNode, int index)
    where TLayoutElement : LayoutItemNode;

/// <summary>
/// 
/// </summary>
public sealed class ViewLayoutElementNodeFactory
{
    internal Dictionary<Type, CreateViewLayoutElement> Factories { get; } = new();
    internal Dictionary<Type, CreateViewLayoutElementAutoId> AutoIdFactories { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLayoutElement"></typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    public ViewLayoutElementNodeFactory Register<TLayoutElement>(CreateViewLayoutElement<TLayoutElement> factory)
        where TLayoutElement : LayoutItemNode
    {
        Factories[typeof(TLayoutElement)] = (items, item) => factory(items, (TLayoutElement)item);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLayoutElement"></typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    public ViewLayoutElementNodeFactory RegisterAutoId<TLayoutElement>(CreateViewLayoutElementAutoId<TLayoutElement> factory)
        where TLayoutElement : LayoutItemNode
    {
        AutoIdFactories[typeof(TLayoutElement)] = (item, index) => factory((TLayoutElement)item, index);
        return this;
    }

    private CreateViewLayoutElement? FindFactory(Type? type)
    {
        if (type is null)
        {
            return null;
        }
        if (Factories.TryGetValue(type, out var modelViewLayoutElementFactory))
        {
            return modelViewLayoutElementFactory;
        }
        return FindFactory(type.GetBaseType());
    }

    private CreateViewLayoutElementAutoId? FindAutoIdFactory(Type? type)
    {
        if (type is null)
        {
            return null;
        }
        if (AutoIdFactories.TryGetValue(type, out var modelViewLayoutElementAutoIdFactory))
        {
            return modelViewLayoutElementAutoIdFactory;
        }
        return FindAutoIdFactory(type.GetBaseType());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="layoutItemNode"></param>
    /// <returns></returns>
    internal IModelViewLayoutElement? CreateViewLayoutElement(IModelNode parentNode, LayoutItemNode layoutItemNode)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="layoutItemNode"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal string? CreateViewLayoutElementAutoId(LayoutItemNode layoutItemNode, int index)
    {
        _ = layoutItemNode ?? throw new ArgumentNullException(nameof(layoutItemNode));

        var builder = FindAutoIdFactory(layoutItemNode.GetType());

        if (builder is not null)
        {
            return builder(layoutItemNode, index);
        }

        return null;
    }
}

/// <summary>
/// Class ModelDetailViewLayoutNodesGeneratorUpdater. Implements the
/// <see cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelDetailViewLayoutNodesGenerator}" />
/// </summary>
///
/// <seealso cref="ModelNodesGeneratorUpdater{ModelDetailViewLayoutNodesGenerator}"/>
/// <seealso cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelDetailViewLayoutNodesGenerator}"> <autogeneratedoc /></seealso>

[XenialCheckLicense]
public sealed partial class XenialDetailViewLayoutNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelDetailViewLayoutNodesGenerator>
{
    /// <summary>
    /// 
    /// </summary>
    public static ViewLayoutElementNodeFactory NodeFactory { get; } = new ViewLayoutElementNodeFactory()
        .Register<LayoutGroupItem>((parentNode, layoutItemNode) =>
        {
            var modelLayoutGroup = parentNode.AddNode<IModelLayoutGroup>(layoutItemNode.Id);

            return modelLayoutGroup;
        })
        .RegisterAutoId<LayoutGroupItem>((layoutItemNode, index) => layoutItemNode.Direction switch
        {
            FlowDirection.Horizontal => $"HGroup-{index}",
            FlowDirection.Vertical => $"VGroup-{index}",
            _ => string.Empty
        })
        .Register<LayoutTabGroupItem>((parentNode, layoutItemNode) =>
        {
            var modelLayoutViewItem = parentNode.AddNode<IModelLayoutGroup>(layoutItemNode.Id);

            return modelLayoutViewItem;
        })
        .RegisterAutoId<LayoutTabbedGroupItem>((layoutItemNode, index) =>
            $"Tab-{index}"
        )
        .Register<LayoutTabbedGroupItem>((parentNode, layoutItemNode) =>
        {
            var modelTabbedGroup = parentNode.AddNode<IModelTabbedGroup>(layoutItemNode.Id);

            return modelTabbedGroup;
        })
        .RegisterAutoId<LayoutTabbedGroupItem>((layoutItemNode, index) =>
            $"Tabs-{index}"
        )
        .Register<LayoutEmptySpaceItem>((parentNode, layoutItemNode) =>
        {
            var modelLayoutViewItem = parentNode.AddNode<IModelLayoutViewItem>(layoutItemNode.Id);

            return modelLayoutViewItem;
        })
        .RegisterAutoId<LayoutEmptySpaceItem>((layoutItemNode, index) =>
            $"EmptySpace-{index}"
        )
        .Register<LayoutViewItem>((parentNode, layoutItemNode) =>
        {
            static IModelViewItems? FindViewItems(IModelNode? modelNode)
            {
                if (modelNode is IModelViews || modelNode is null) //Stop here, we don't need to lookup any further
                {
                    return null;
                }

                if (modelNode is IModelCompositeView modelCompositeView)
                {
                    return modelCompositeView.Items;
                }

                return FindViewItems(modelNode.Parent);
            }

            var modelLayoutViewItem = parentNode.AddNode<IModelLayoutViewItem>(layoutItemNode.Id);

            var viewItems = FindViewItems(parentNode);
            if (viewItems is not null)
            {
                if (layoutItemNode.IsDuplicate)
                {
                    //TODO: Better duplicated node handler
                    var viewItem = viewItems.OfType<IModelViewItem>().FirstOrDefault(m => m.Id == layoutItemNode.Id);
                    modelLayoutViewItem.ViewItem = viewItem;
                }
                else
                {
                    var viewItem = viewItems.OfType<IModelViewItem>().FirstOrDefault(m => m.Id == layoutItemNode.ViewItemId);
                    modelLayoutViewItem.ViewItem = viewItem;
                }
            }

            return modelLayoutViewItem;
        })
        .RegisterAutoId<LayoutViewItem>((layoutItemNode, index) =>
            $"ViewItem-{index}"
        )
        ;

    internal static BuildLayoutFunctor? FindFunctor(IModelDetailView modelDetailView)
    {
        var layoutBuilderAttributes = modelDetailView.ModelClass.TypeInfo.FindAttributes<DetailViewLayoutBuilderAttribute>();
        foreach (var attribute in layoutBuilderAttributes)
        {
            var targetViewId =
                string.IsNullOrEmpty(attribute.ViewId)
                ? modelDetailView.ModelClass.DefaultDetailView?.Id
                : attribute.ViewId;

            if (string.IsNullOrEmpty(targetViewId))
            {
                targetViewId = ModelNodeIdHelper.GetDetailViewId(modelDetailView.ModelClass.TypeInfo.Type);
            }

            if (modelDetailView.Id == targetViewId)
            {
                if (!string.IsNullOrEmpty(attribute.BuildLayoutMethodName))
                {
                    if (attribute.GeneratorType is null)
                    {
                        attribute.GeneratorType = modelDetailView.ModelClass.TypeInfo.Type;
                    }
                }

                if (attribute.BuildLayoutDelegate is null)
                {
                    if (string.IsNullOrEmpty(attribute.BuildLayoutMethodName))
                    {
                        attribute.BuildLayoutMethodName = "BuildLayout";
                        if (attribute.GeneratorType is null)
                        {
                            attribute.GeneratorType = modelDetailView.ModelClass.TypeInfo.Type;
                        }
                    }

                    if (attribute.GeneratorType is not null)
                    {
                        var method = attribute.GeneratorType.GetMethod(attribute.BuildLayoutMethodName);
                        if (method is not null)
                        {
                            if (method.IsStatic)
                            {
                                var @delegate = Delegate.CreateDelegate(typeof(BuildLayoutFunctor), method);
                                attribute.BuildLayoutDelegate = (BuildLayoutFunctor)@delegate;
                            }
                            else
                            {
                                //TODO: Cleanup instance and factory
                                var generatorInstance = Activator.CreateInstance(attribute.GeneratorType);

                                var @delegate = Delegate.CreateDelegate(typeof(BuildLayoutFunctor), generatorInstance, method);
                                attribute.BuildLayoutDelegate = (BuildLayoutFunctor)@delegate;
                            }
                        } //TODO: ERROR HANDLING
                    }
                }

                //TODO: Factory
                if (attribute.BuildLayoutDelegate is not null)
                {
                    return attribute.BuildLayoutDelegate;
                }
            }
        }
        return null;
    }

    internal static Layout InvokeBuilder(BuildLayoutFunctor builder, IModelDetailView modelDetailView)
        => builder.Invoke()
           ?? throw new InvalidOperationException($"LayoutBuilder on Type '{modelDetailView.ModelClass.TypeInfo.Type}' for View '{modelDetailView.Id}' must return an object of Type '{typeof(Layout)}'");

    internal static void MarkDuplicateNodes(Layout layout)
    {
        var duplicatedIds = VisitNodes<LayoutViewItem>(layout)
            .GroupBy(i => i.Id)
            .Where(i => i.Count() > 1)
            .Select(i => (i.Key, i.ToList()));

        foreach (var (id, duplicates) in duplicatedIds)
        {
            var i = 1;
            foreach (var duplicate in duplicates.Skip(1).ToList())
            {
                duplicate.Id = $"{duplicate.Id}{i}";
                duplicate.IsDuplicate = true;
                i++;
            }
        }
    }

    /// <summary>
    /// Updates the Application Model node content generated by the Nodes Generator, specified by the
    /// <see cref="T:DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater`1" /> class' type
    /// parameter.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
    ///                                                 invalid. </exception>
    ///
    /// <value> The ca 1725. </value>

#pragma warning disable CA1725 //match identifier of base class -> would conflict with nodes
    public override void UpdateNode(ModelNode modelNode)
#pragma warning restore CA1725 //match identifier of base class -> would conflict with nodes
    {
        if (modelNode is IModelViewLayout modelViewLayout)
        {
            if (modelViewLayout.Parent is IModelDetailView modelDetailView)
            {
                var builder = FindFunctor(modelDetailView);
                if (builder is null)
                {
                    return;
                }

                var layout = InvokeBuilder(builder, modelDetailView);

                modelViewLayout.ClearNodes();

                var modelMainNode = modelViewLayout
                    .AddNode<IModelLayoutGroup>(ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName)
                    ?? throw new InvalidOperationException($"Cannot generate 'Main' node on Type '{modelDetailView.ModelClass.TypeInfo.Type}' for View '{modelDetailView.Id}'");

                MarkDuplicateNodes(layout);

                var currentIndex = 0;
                foreach (var layoutItemNode in layout)
                {
                    var (el, newIndex, node) = FactorNodes(NodeFactory, currentIndex, modelMainNode, layoutItemNode);
                    currentIndex = newIndex + 1;
                    if (el is not null)
                    {
                        AutoFactorName(NodeFactory, el, newIndex, node);
                    }

                    static void AutoFactorName(ViewLayoutElementNodeFactory nodeFactory, IModelViewLayoutElement el, int index, LayoutItemNode n)
                    {
                        if (string.IsNullOrEmpty(n.Id))
                        {
                            var newId = nodeFactory.CreateViewLayoutElementAutoId(n, index);
                            if (!string.IsNullOrEmpty(newId))
                            {
                                el.Id = newId;
                            }
                        }
                    }

                    static (IModelViewLayoutElement? el, int index, LayoutItemNode n) FactorNodes(ViewLayoutElementNodeFactory nodeFactory, int index, IModelNode parentNode, LayoutItemNode layoutItemNode)
                    {
                        var node = nodeFactory.CreateViewLayoutElement(parentNode, layoutItemNode);
                        if (node is not null && node.Index is null)
                        {
                            node.Index = index;
                        }
                        if (layoutItemNode is IEnumerable<LayoutItemNode> layoutNodeWithChildren
                            && node is not null)
                        {
                            var xIndex = 0;
                            foreach (var childNode in layoutNodeWithChildren)
                            {
                                var (n, cI, factoredNode) = FactorNodes(nodeFactory, xIndex, node, childNode);
                                if (n is not null && n.Index is null)
                                {
                                    n.Index = cI;
                                }
                                if (n is not null)
                                {
                                    AutoFactorName(nodeFactory, n, cI, factoredNode);
                                }
                                xIndex = cI + 1;
                            }
                        }
                        new LayoutItemMapper().Map(layoutItemNode, node);
                        return (node, node?.Index ?? 0, layoutItemNode);
                    }
                }
            }
        }
    }

}
