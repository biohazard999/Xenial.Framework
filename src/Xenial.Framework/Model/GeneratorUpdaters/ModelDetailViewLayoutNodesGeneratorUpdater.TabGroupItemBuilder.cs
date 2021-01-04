using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items;

using static Xenial.Framework.Model.GeneratorUpdaters.ModelDetailViewLayoutNodesGeneratorUpdaterMappers;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    public partial class ModelDetailViewLayoutNodesGeneratorUpdater
    {
        internal class LayoutTabGroupItemBuilder
            : ModelViewLayoutElementFactory<IModelLayoutGroup, LayoutTabGroupItem>
        {
            protected override IModelLayoutGroup? CreateViewLayoutElement(IModelNode parentNode, LayoutTabGroupItem tabGroupItemNode)
            {
                var modelLayoutViewItem = parentNode.AddNode<IModelLayoutGroup>(tabGroupItemNode.Id);

                if (modelLayoutViewItem is IModelNode genericModelNode)
                {
                    MapModelNode(genericModelNode, tabGroupItemNode);
                }

                if (modelLayoutViewItem is IModelViewLayoutElement modelViewLayoutElement)
                {
                    MapModelViewLayoutElement(modelViewLayoutElement, tabGroupItemNode);
                }

                if (modelLayoutViewItem is IModelLayoutElementWithCaptionOptions modelLayoutElementWithCaptionOptions)
                {
                    MapLayoutElementWithCaptionOptions(modelLayoutElementWithCaptionOptions, tabGroupItemNode);
                }

                if (modelLayoutViewItem is IModelLayoutElementWithCaption modelLayoutElementWithCaption)
                {
                    MapCaption(modelLayoutElementWithCaption, tabGroupItemNode);
                }

                if (modelLayoutViewItem is ISupportControlAlignment modelSupportControlAlignment)
                {
                    MapSupportControlAlignment(modelSupportControlAlignment, tabGroupItemNode);
                }

                if (modelLayoutViewItem is IModelToolTip modelToolTip)
                {
                    MapModelToolTip(modelToolTip, tabGroupItemNode);
                }

                if (modelLayoutViewItem is IModelToolTipOptions modelToolTipOptions)
                {
                    MapModelToolTipOptions(modelToolTipOptions, tabGroupItemNode);
                }

                MapLayoutGroup(modelLayoutViewItem, tabGroupItemNode);

                if (tabGroupItemNode.LayoutGroupOptions is not null)
                {
                    tabGroupItemNode.LayoutGroupOptions(modelLayoutViewItem);
                }

                return modelLayoutViewItem;
            }
        }
    }
}
