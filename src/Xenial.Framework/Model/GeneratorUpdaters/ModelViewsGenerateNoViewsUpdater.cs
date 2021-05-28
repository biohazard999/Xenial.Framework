﻿using System;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Framework.Base;

#pragma warning disable CA1309 //Use ordinal string comparison -> By Design
#pragma warning disable CA1307 //Use ordinal string comparison -> By Design

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    /// <summary>
    /// Class ModelViewsGenerateNoViewsUpdater. This class cannot be inherited. Implements the
    /// <see cref="ModelNodesGeneratorUpdater{ModelViewsNodesGenerator}" />
    /// </summary>
    ///
    /// <seealso cref="ModelNodesGeneratorUpdater{ModelViewsNodesGenerator}">
    /// <autogeneratedoc />
    /// </seealso>

    [XenialCheckLicence]
    public sealed partial class ModelViewsGenerateNoViewsUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>
    {
        /// <summary>
        /// Updates the Application Model node content generated by the Nodes Generator, specified by the
        /// <see cref="T:DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater`1" /> class' type
        /// parameter.
        /// </summary>
        ///
        /// <param name="node"> A ModelNode Application Model node to be updated. </param>

        public override void UpdateNode(ModelNode node)
        {
            if (node is IModelViews views)
            {
                //Make sure we generate navigation item nodes before we remove the views
                //Otherwise we have no idea what View/ModelClass the navigation item belonged to
                if (views.Application is IModelApplicationNavigationItems modelApplicationNavigationItems)
                {
                    _ = modelApplicationNavigationItems.NavigationItems.Items;
                }

                foreach (var view in views.OfType<IModelObjectView>())
                {
                    foreach (var attribute in view.ModelClass.TypeInfo.FindAttributes<GenerateNoViewAttribute>())
                    {
                        if (view.Id.Equals(attribute.ViewId))
                        {
                            view.Remove();
                        }
                    }

                    if (view is IModelDetailView)
                    {
                        if (view.ModelClass.TypeInfo.IsAttributeDefined<GenerateNoDetailViewAttribute>(false))
                        {
                            if (ModelNodeIdHelper.GetDetailViewId(view.ModelClass.TypeInfo.Type).Equals(view.Id))
                            {
                                view.Remove();
                            }
                        }
                    }

                    if (view is IModelListView)
                    {
                        if (view.ModelClass.TypeInfo.IsAttributeDefined<GenerateNoListViewAttribute>(false))
                        {
                            if (ModelNodeIdHelper.GetListViewId(view.ModelClass.TypeInfo.Type).Equals(view.Id))
                            {
                                view.Remove();
                            }
                        }

                        if (view.ModelClass.TypeInfo.IsAttributeDefined<GenerateNoLookupListViewAttribute>(false))
                        {
                            if (ModelNodeIdHelper.GetLookupListViewId(view.ModelClass.TypeInfo.Type).Equals(view.Id))
                            {
                                view.Remove();
                            }
                        }
                    }
                }
            }
        }
    }
}
