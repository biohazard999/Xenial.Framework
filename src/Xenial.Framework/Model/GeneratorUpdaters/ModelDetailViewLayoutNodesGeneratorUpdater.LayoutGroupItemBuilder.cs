﻿using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items;

namespace Xenial.Framework.Model.GeneratorUpdaters;

public partial class ModelDetailViewLayoutNodesGeneratorUpdater
{
    internal class LayoutGroupItemBuilder
        : ModelViewLayoutElementFactory<IModelLayoutGroup, LayoutGroupItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutItemNode"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string? CreateAutoGeneratedId(LayoutGroupItem layoutItemNode, int index)
            => layoutItemNode.Direction switch
            {
                FlowDirection.Horizontal => $"HGroup-{index}",
                FlowDirection.Vertical => $"VGroup-{index}",
                _ => null
            };

        /// <summary>   Creates view layout element. </summary>
        ///
        /// <param name="parentNode">       The parent node. </param>
        /// <param name="groupItemNode">    The group item node. </param>
        ///
        /// <returns>   The new view layout element. </returns>

        protected override IModelLayoutGroup? CreateViewLayoutElement(IModelNode parentNode, LayoutGroupItem groupItemNode)
        {
            var modelLayoutGroup = parentNode.AddNode<IModelLayoutGroup>(groupItemNode.Id);

            return modelLayoutGroup;
        }
    }
}
