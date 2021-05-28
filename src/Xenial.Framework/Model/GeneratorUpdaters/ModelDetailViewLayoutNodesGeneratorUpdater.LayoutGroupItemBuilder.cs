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
        internal class LayoutGroupItemBuilder
            : ModelViewLayoutElementFactory<IModelLayoutGroup, LayoutGroupItem>
        {
            /// <summary>   Creates view layout element. </summary>
            ///
            /// <param name="parentNode">       The parent node. </param>
            /// <param name="groupItemNode">    The group item node. </param>
            ///
            /// <returns>   The new view layout element. </returns>

            protected override IModelLayoutGroup? CreateViewLayoutElement(IModelNode parentNode, LayoutGroupItem groupItemNode)
            {
                var modelLayoutGroup = parentNode.AddNode<IModelLayoutGroup>(groupItemNode.Id);

                if (modelLayoutGroup is IModelNode genericModelNode)
                {
                    MapModelNode(genericModelNode, groupItemNode);
                }

                if (modelLayoutGroup is IModelViewLayoutElement modelViewLayoutElement)
                {
                    MapModelViewLayoutElement(modelViewLayoutElement, groupItemNode);
                }

                if (modelLayoutGroup is IModelLayoutElementWithCaptionOptions modelLayoutElementWithCaptionOptions)
                {
                    MapLayoutElementWithCaptionOptions(modelLayoutElementWithCaptionOptions, groupItemNode);
                }

                if (modelLayoutGroup is IModelLayoutElementWithCaption modelLayoutElementWithCaption)
                {
                    MapCaption(modelLayoutElementWithCaption, groupItemNode);
                }

                if (modelLayoutGroup is ISupportControlAlignment modelSupportControlAlignment)
                {
                    MapSupportControlAlignment(modelSupportControlAlignment, groupItemNode);
                }

                if (modelLayoutGroup is IModelToolTip modelToolTip)
                {
                    MapModelToolTip(modelToolTip, groupItemNode);
                }

                if (modelLayoutGroup is IModelToolTipOptions modelToolTipOptions)
                {
                    MapModelToolTipOptions(modelToolTipOptions, groupItemNode);
                }

                MapLayoutGroup(modelLayoutGroup, groupItemNode);

                if (groupItemNode.LayoutGroupOptions is not null)
                {
                    groupItemNode.LayoutGroupOptions(modelLayoutGroup);
                }

                return modelLayoutGroup;
            }
        }
    }
}
