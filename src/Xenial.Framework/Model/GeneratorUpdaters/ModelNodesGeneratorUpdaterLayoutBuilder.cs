﻿using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    /// <summary>
    /// Class ModelNodesGeneratorUpdaterLayoutBuilder.
    /// Implements the <see cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelViewsNodesGenerator}" />
    /// </summary>
    /// <seealso cref="DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater{DevExpress.ExpressApp.Model.NodeGenerators.ModelViewsNodesGenerator}" />
    /// <autogeneratedoc />
    [XenialCheckLicence]
    public sealed partial class ModelNodesGeneratorUpdaterLayoutBuilder : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>
    {
        /// <summary>
        /// Gets or sets a value indicating whether [build layouts delayed].
        /// </summary>
        /// <value><c>true</c> if [build layouts delayed]; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public bool BuildLayoutsDelayed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic generate missing view items].
        /// </summary>
        /// <value><c>true</c> if [automatic generate missing view items]; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public bool AutoGenerateMissingDetailViewItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [remove unused view items].
        /// </summary>
        /// <value><c>true</c> if [remove unused view items]; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public bool RemoveUnusedDetailViewItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [automatic generate missing views].
        /// </summary>
        /// <value><c>true</c> if [automatic generate missing views]; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public bool AutoGenerateMissingViews { get; set; } = true;

        /// <summary>
        /// Updates the Application Model node content generated by the Nodes Generator, specified by the <see cref="T:DevExpress.ExpressApp.Model.ModelNodesGeneratorUpdater`1" /> class' type parameter.
        /// </summary>
        /// <param name="node">A ModelNode Application Model node to be updated.</param>
        /// <autogeneratedoc />
        public override void UpdateNode(ModelNode node)
        {
            if (node is IModelDetailView modelDetailView)
            {
                if (AutoGenerateMissingDetailViewItems)
                {

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
