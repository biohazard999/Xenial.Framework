using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.Pdf.Native;

using Xenial.Framework.Layouts.Items;

using static Xenial.Framework.Model.GeneratorUpdaters.ModelDetailViewLayoutNodesGeneratorUpdaterMappers;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    public partial class ModelDetailViewLayoutNodesGeneratorUpdater
    {
        internal class TabbedGroupItemBuilder
            : ModelViewLayoutElementFactory<IModelTabbedGroup, LayoutTabbedGroupItem>
        {
            protected override IModelTabbedGroup? CreateViewLayoutElement(IModelNode parentNode, LayoutTabbedGroupItem tabbedGroupItemNode)
            {
                var modelTabbedGroup = parentNode.AddNode<IModelTabbedGroup>(tabbedGroupItemNode.Id);

                if (modelTabbedGroup is IModelNode genericModelNode)
                {
                    MapModelNode(genericModelNode, tabbedGroupItemNode);
                }

                if (modelTabbedGroup is IModelViewLayoutElement modelViewLayoutElement)
                {
                    MapModelViewLayoutElement(modelViewLayoutElement, tabbedGroupItemNode);
                }

                if (modelTabbedGroup is IModelLayoutElementWithCaptionOptions modelLayoutElementWithCaptionOptions)
                {
                    MapLayoutElementWithCaptionOptions(modelLayoutElementWithCaptionOptions, tabbedGroupItemNode);
                }

                if (modelTabbedGroup is IModelLayoutElementWithCaption modelLayoutElementWithCaption)
                {
                    MapCaption(modelLayoutElementWithCaption, tabbedGroupItemNode);
                }

                MapTabbedLayoutGroup(modelTabbedGroup, tabbedGroupItemNode);

                if (tabbedGroupItemNode.TabbedGroupOptions is not null)
                {
                    tabbedGroupItemNode.TabbedGroupOptions(modelTabbedGroup);
                }

                return modelTabbedGroup;
            }
        }
    }
}
