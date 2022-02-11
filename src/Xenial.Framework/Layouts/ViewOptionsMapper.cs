using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Layouts;

[XenialModelOptionsMapper(typeof(DetailViewOptions))]
[XenialModelOptionsMapper(typeof(ListViewOptions))]
internal partial class ViewOptionsMapper
{
    partial void MapNodeCore(DetailViewOptions from, IModelDetailView to)
    {
        foreach (var option in from.ExtensionsCollection.AsEnumerable())
        {
            MappingFactory.Factory.MapDetailViewOptions(option, to);
        }
    }

    partial void MapNodeCore(ListViewOptions from, IModelListView to)
    {
        foreach (var option in from.ExtensionsCollection)
        {
            MappingFactory.Factory.MapListViewOptions(option, to);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genericOptions"></param>
    /// <param name="node"></param>
    public static void MapGenericOptions(GenericDetailViewOptions genericOptions, IModelNode node)
    {
        if (node is ModelNode modelNode)
        {
            foreach (var item in genericOptions.AsEnumerable())
            {
                modelNode.SetValue(item.Key, item.Value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genericOptions"></param>
    /// <param name="node"></param>
    public static void MapGenericOptions(GenericListViewOptions genericOptions, IModelNode node)
    {
        if (node is ModelNode modelNode)
        {
            foreach (var item in genericOptions)
            {
                modelNode.SetValue(item.Key, item.Value);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="hiddenActionOptions"></param>
    /// <param name="node"></param>
    public static void MapHiddenActions(HiddenActionsOptions hiddenActionOptions, IModelNode node)
    {
        if (node is IModelDetailView modelDetailView && modelDetailView is IModelViewHiddenActions modelViewHiddenActions)
        {
            foreach (var actionId in hiddenActionOptions)
            {
                var action = modelViewHiddenActions.HiddenActions.AddNode<IModelActionLink>(actionId);
                action.ActionId = actionId;
            }
        }
    }
}
