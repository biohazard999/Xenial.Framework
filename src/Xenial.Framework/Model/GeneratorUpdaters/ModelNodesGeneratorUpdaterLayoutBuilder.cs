﻿using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// Class ModelNodesGeneratorUpdaterLayoutBuilder. Implements the
/// <see cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelViewsNodesGenerator}" />
/// </summary>
///
/// <seealso cref="ModelNodesGeneratorUpdater{ModelViewsNodesGenerator}"/>
/// <seealso cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelViewsNodesGenerator}">    <autogeneratedoc /></seealso>

[XenialCheckLicense]
public sealed partial class ModelNodesGeneratorUpdaterLayoutBuilder : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>
{
    /// <summary>   Gets or sets a value indicating whether [build layouts delayed]. </summary>
    ///
    /// <value> <c>true</c> if [build layouts delayed]; otherwise, <c>false</c>. </value>

    public bool BuildLayoutsDelayed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic generate missing view items].
    /// </summary>
    ///
    /// <value>
    /// <c>true</c> if [automatic generate missing view items]; otherwise, <c>false</c>.
    /// </value>

    public bool AutoGenerateMissingDetailViewItems { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether [remove unused view items]. </summary>
    ///
    /// <value> <c>true</c> if [remove unused view items]; otherwise, <c>false</c>. </value>

    public bool RemoveUnusedDetailViewItems { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [automatic generate missing views].
    /// </summary>
    ///
    /// <value> <c>true</c> if [automatic generate missing views]; otherwise, <c>false</c>. </value>

    public bool AutoGenerateMissingViews { get; set; } = true;

    /// <summary>
    /// Updates the Application Model node content generated by the Nodes Generator, specified by the
    /// <see cref="T:DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater`1" /> class' type
    /// parameter.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
    ///                                                 invalid. </exception>
    ///
    /// <param name="node"> A ModelNode Application Model node to be updated. </param>

    public override void UpdateNode(ModelNode node)
    {
        if (node is IModelViews modelViews)
        {
            if (AutoGenerateMissingDetailViewItems)
            {
                foreach (var possibleModelDetailView in modelViews.OfType<IModelDetailView>())
                {
                    var attribute = possibleModelDetailView.ModelClass.TypeInfo.FindAttribute<DetailViewLayoutBuilderAttribute>();
                    //TODO: Factory
                    if (attribute is not null && attribute.BuildLayoutDelegate is not null)
                    {
                        var builder = attribute.BuildLayoutDelegate;
                        var layout = builder.Invoke()
                            ?? throw new InvalidOperationException($"LayoutBuilder on Type '{possibleModelDetailView.ModelClass.TypeInfo.Type}' for View '{possibleModelDetailView.Id}' must return an object of Type '{typeof(Layout)}'");

                        foreach (var layoutViewItemNode in VisitNodes<LayoutViewItem>(layout))
                        {
                            var modelViewItemNode = possibleModelDetailView.Items.FirstOrDefault(m => m.Id == layoutViewItemNode.Id);

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
                    if (item is TItem tItem)
                    {
                        yield return tItem;
                    }

                    foreach (var nestedItem in VisitNodes<TItem>(item))
                    {
                        yield return nestedItem;
                    }
                }
            }
        }
    }
}
