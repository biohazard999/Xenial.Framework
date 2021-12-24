﻿using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
public sealed partial class ModelDetailViewLayoutModelDetailViewItemsNodesGenerator : ModelNodesGeneratorUpdater<ModelDetailViewItemsNodesGenerator>
{
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
                var builder = ModelDetailViewLayoutNodesGeneratorUpdater.FindFunctor(modelDetailView);
                if (builder is null)
                {
                    return;
                }

                var layout = ModelDetailViewLayoutNodesGeneratorUpdater.InvokeBuilder(builder, modelDetailView);

                ModelDetailViewLayoutNodesGeneratorUpdater.MarkDuplicateNodes(layout);

                foreach (var layoutViewItemNode in VisitNodes<LayoutPropertyEditorItem>(layout))
                {
                    var viewItem = viewItems.OfType<IModelViewItem>().FirstOrDefault(m =>
                        m.Id == (layoutViewItemNode.IsDuplicate
                        ? layoutViewItemNode.Id
                        : layoutViewItemNode.ViewItemId)
                    );

                    if (viewItem is null)
                    {
                        var newViewItem = viewItems.AddNode<IModelPropertyEditor>(layoutViewItemNode.Id);

                        newViewItem.PropertyName = layoutViewItemNode.ViewItemId;
                        ////For whatever reason we need to reset the editor here
                        ////may be removed if we switch this code to the IModelViewItems node builder
                        //if (newViewItem is IModelPropertyEditor oldPropertyEditor)
                        //{
                        //    //newViewItem.ClearValue(nameof(newViewItem.PropertyEditorType));
                        //    //newViewItem.PropertyEditorType = typeof(DevExpress.ExpressApp.Editors.DetailPropertyEditor);
                        //}
                    }
                }

                foreach (var layoutViewItemNode in VisitNodes<LayoutViewItem>(layout))
                {
                    var modelViewItemNode = viewItems.FirstOrDefault(m => m.Id == layoutViewItemNode.Id);

                    //This is the most weird side effect so far.
                    //For whatever reason we need to specify the caption
                    //to generate a deterministic model tree
                    if (modelViewItemNode is not null)
                    {
                        modelViewItemNode.Caption =
                            string.IsNullOrEmpty(layoutViewItemNode.Caption)
                            ? modelViewItemNode.Caption
                            : layoutViewItemNode.Caption;
                    }
                }
            }
        }

        static IEnumerable<TItem> VisitNodes<TItem>(LayoutItemNode node)
            where TItem : LayoutItemNode
        {
            if (node is TItem targetNode)
            {
                yield return targetNode;
            }

            if (node is IEnumerable<LayoutItemNode> items)
            {
                foreach (var item in items)
                {
                    foreach (var nestedItem in VisitNodes<TItem>(item))
                    {
                        yield return nestedItem;
                    }
                }
            }
        }
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
public sealed partial class ModelDetailViewLayoutNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelDetailViewLayoutNodesGenerator>
{
    private readonly NodeBuilderFactory nodeBuilderFactory
        = new NodeBuilderFactory()
            .Register<LayoutGroupItem, LayoutGroupItemBuilder>(() => new LayoutGroupItemBuilder())
            .Register<LayoutTabGroupItem, LayoutTabGroupItemBuilder>(() => new LayoutTabGroupItemBuilder())
            .Register<LayoutTabbedGroupItem, TabbedGroupItemBuilder>(() => new TabbedGroupItemBuilder())
            .Register<LayoutEmptySpaceItem, EmptySpaceItemBuilder>(() => new EmptySpaceItemBuilder())
            .Register<LayoutViewItem, LayoutViewItemBuilder>(() => new LayoutViewItemBuilder())
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
        var duplicatedIds = VisitNodes<LayoutPropertyEditorItem>(layout)
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

    private static IEnumerable<TItem> VisitNodes<TItem>(LayoutItemNode node)
            where TItem : LayoutItemNode
    {
        if (node is TItem targetNode)
        {
            yield return targetNode;
        }

        if (node is IEnumerable<LayoutItemNode> items)
        {
            foreach (var item in items)
            {
                foreach (var nestedItem in VisitNodes<TItem>(item))
                {
                    yield return nestedItem;
                }
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
        _ = nodeBuilderFactory ?? throw new InvalidOperationException();

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
                    var (el, newIndex, node) = FactorNodes(nodeBuilderFactory, currentIndex, modelMainNode, layoutItemNode);
                    currentIndex = newIndex + 1;
                    if (el is not null)
                    {
                        AutoFactorName(nodeBuilderFactory, el, newIndex, node);
                    }

                    static void AutoFactorName(NodeBuilderFactory nodeBuilderFactory, IModelViewLayoutElement el, int index, LayoutItemNode n)
                    {
                        if (string.IsNullOrEmpty(n.Id))
                        {
                            var newId = nodeBuilderFactory.CreateAutoGeneratedId(n, index);
                            if (!string.IsNullOrEmpty(newId))
                            {
                                el.Id = newId;
                            }
                        }
                    }

                    static (IModelViewLayoutElement? el, int index, LayoutItemNode n) FactorNodes(NodeBuilderFactory nodeBuilderFactory, int index, IModelNode parentNode, LayoutItemNode layoutItemNode)
                    {
                        var node = nodeBuilderFactory.CreateViewLayoutElement(parentNode, layoutItemNode);
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
                                var (n, cI, factoredNode) = FactorNodes(nodeBuilderFactory, xIndex, node, childNode);
                                if (n is not null && n.Index is null)
                                {
                                    n.Index = cI;
                                }
                                if (n is not null)
                                {
                                    AutoFactorName(nodeBuilderFactory, n, cI, factoredNode);
                                }
                                xIndex = cI + 1;
                            }
                        }
                        return (node, node?.Index ?? 0, layoutItemNode);
                    }
                }
            }
        }
    }

}
