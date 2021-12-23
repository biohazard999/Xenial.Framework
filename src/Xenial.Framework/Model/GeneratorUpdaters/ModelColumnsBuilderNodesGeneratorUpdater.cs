﻿using System;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.Layouts;

using static Xenial.Framework.Model.GeneratorUpdaters.ModelColumnsBuilderNodesGeneratorUpdaterMappers;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// Class ModelColumnsBuilderNodesGeneratorUpdater. This class cannot be inherited. Implements the
/// <see cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelListViewColumnsNodesGenerator}" />
/// </summary>
///
/// <seealso cref="ModelNodesGeneratorUpdater{ModelListViewColumnsNodesGenerator}"/>
/// <seealso cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelListViewColumnsNodesGenerator}">  <autogeneratedoc /></seealso>

[XenialCheckLicense]
public sealed partial class ModelColumnsBuilderNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelListViewColumnsNodesGenerator>
{
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
        if (node is IModelColumns modelColumns)
        {
            if (modelColumns.Parent is IModelListView modelListView)
            {
                var columnBuilderAttributes = modelListView.ModelClass.TypeInfo.FindAttributes<ColumnsBuilderAttribute>();

                foreach (var attribute in columnBuilderAttributes)
                {
                    var targetViewId =
                        string.IsNullOrEmpty(attribute.ViewId)
                        ?
                            (attribute is ListViewColumnsBuilderAttribute
                             ? modelListView.ModelClass.DefaultListView?.Id
                             : modelListView.ModelClass.DefaultLookupListView?.Id
                            )
                        : attribute.ViewId;

                    if (string.IsNullOrEmpty(targetViewId))
                    {
                        targetViewId = attribute is ListViewColumnsBuilderAttribute
                            ? ModelNodeIdHelper.GetListViewId(modelListView.ModelClass.TypeInfo.Type)
                            : ModelNodeIdHelper.GetLookupListViewId(modelListView.ModelClass.TypeInfo.Type);
                    }

                    if (modelListView.Id == targetViewId)
                    {
                        if (!string.IsNullOrEmpty(attribute.BuildColumnsMethodName))
                        {
                            if (attribute.GeneratorType is null)
                            {
                                attribute.GeneratorType = modelListView.ModelClass.TypeInfo.Type;
                            }
                        }

                        if (attribute.BuildColumnsDelegate is null)
                        {
                            if (string.IsNullOrEmpty(attribute.BuildColumnsMethodName))
                            {
                                attribute.BuildColumnsMethodName = attribute is ListViewColumnsBuilderAttribute
                                    ? "BuildColumns"
                                    : "BuildLookupColumns";

                                if (attribute.GeneratorType is null)
                                {
                                    attribute.GeneratorType = modelListView.ModelClass.TypeInfo.Type;
                                }
                            }

                            if (attribute.GeneratorType is not null)
                            {
                                var method = attribute.GeneratorType.GetMethod(attribute.BuildColumnsMethodName);
                                if (method is not null)
                                {
                                    var @delegate = Delegate.CreateDelegate(typeof(BuildColumnsFunctor), method);
                                    attribute.BuildColumnsDelegate = (BuildColumnsFunctor)@delegate;
                                } //TODO: ERROR HANDLING
                            }
                        }

                        //TODO: Factory
                        if (attribute.BuildColumnsDelegate is not null)
                        {
                            var builder = attribute.BuildColumnsDelegate;
                            var columns = builder.Invoke()
                                ?? throw new InvalidOperationException($"ColumnsBuilder on Type '{modelListView.ModelClass.TypeInfo.Type}' for View '{modelListView.Id}' must return an object of Type '{typeof(Columns)}'");

                            modelColumns.ClearNodes();

                            var index = 0;
                            foreach (var column in columns)
                            {
                                var columnNode = modelColumns.AddNode<IModelColumn>(column.Id);
                                columnNode.Index = index;
                                MapColumn(columnNode, column);
                                MapModelMemberViewItem(columnNode, column);
                                MapModelLayoutElement(columnNode, column);
                                MapModelToolTip(columnNode, column);
                                MapModelCommonMemberViewItem(columnNode, column);

                                if (column.Index.HasValue)
                                {
                                    index = column.Index.Value + 1;
                                }
                                else
                                {
                                    index++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
