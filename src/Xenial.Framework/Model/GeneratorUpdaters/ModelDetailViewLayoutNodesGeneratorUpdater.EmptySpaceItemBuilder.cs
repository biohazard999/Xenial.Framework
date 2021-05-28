using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.LeafNodes;

using static Xenial.Framework.Model.GeneratorUpdaters.ModelDetailViewLayoutNodesGeneratorUpdaterMappers;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    public partial class ModelDetailViewLayoutNodesGeneratorUpdater
    {
        internal class EmptySpaceItemBuilder
            : ModelViewLayoutElementFactory<IModelLayoutViewItem, LayoutEmptySpaceItem>
        {
            /// <summary>   Creates view layout element. </summary>
            ///
            /// <param name="parentNode">           The parent node. </param>
            /// <param name="emptySpaceItemNode">   The empty space item node. </param>
            ///
            /// <returns>   The new view layout element. </returns>

            protected override IModelLayoutViewItem? CreateViewLayoutElement(IModelNode parentNode, LayoutEmptySpaceItem emptySpaceItemNode)
            {
                var modelLayoutViewItem = parentNode.AddNode<IModelLayoutViewItem>(emptySpaceItemNode.Id);

                if (modelLayoutViewItem is IModelNode genericModelNode)
                {
                    MapModelNode(genericModelNode, emptySpaceItemNode);
                }

                if (modelLayoutViewItem is IModelLayoutItem modelLayoutItem)
                {
                    MapModelLayoutItem(modelLayoutItem, emptySpaceItemNode);
                }

                if (modelLayoutViewItem is IModelViewLayoutElement modelViewLayoutElement)
                {
                    MapModelViewLayoutElement(modelViewLayoutElement, emptySpaceItemNode);
                }

                if (modelLayoutViewItem is ISupportControlAlignment modelSupportControlAlignment)
                {
                    MapSupportControlAlignment(modelSupportControlAlignment, emptySpaceItemNode);
                }

                return modelLayoutViewItem;
            }
        }
    }
}
