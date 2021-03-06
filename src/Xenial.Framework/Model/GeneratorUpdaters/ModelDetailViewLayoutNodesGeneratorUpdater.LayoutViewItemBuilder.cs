﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

using static Xenial.Framework.Model.GeneratorUpdaters.ModelDetailViewLayoutNodesGeneratorUpdaterMappers;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    public partial class ModelDetailViewLayoutNodesGeneratorUpdater
    {
        internal class LayoutViewItemBuilder
             : ModelViewLayoutElementFactory<IModelLayoutViewItem, LayoutViewItem>
        {
            private static IModelViewItems? FindViewItems(IModelNode? modelNode)
            {
                if (modelNode is IModelViews || modelNode is null) //Stop here, we don't need to lookup any further
                {
                    return null;
                }

                if (modelNode is IModelCompositeView modelCompositeView)
                {
                    return modelCompositeView.Items;
                }

                return FindViewItems(modelNode.Parent);
            }

            /// <summary>   Creates view layout element. </summary>
            ///
            /// <param name="parentNode">           The parent node. </param>
            /// <param name="layoutViewItemNode">   The layout view item node. </param>
            ///
            /// <returns>   The new view layout element. </returns>

            protected override IModelLayoutViewItem? CreateViewLayoutElement(IModelNode parentNode, LayoutViewItem layoutViewItemNode)
            {
                var modelLayoutViewItem = parentNode.AddNode<IModelLayoutViewItem>(layoutViewItemNode.Id);

                var viewItems = FindViewItems(parentNode);
                if (viewItems is not null)
                {
                    modelLayoutViewItem.ViewItem = viewItems.OfType<IModelViewItem>().FirstOrDefault(m => m.Id == layoutViewItemNode.ViewItemId);
                }

                if (modelLayoutViewItem is IModelNode genericModelNode)
                {
                    MapModelNode(genericModelNode, layoutViewItemNode);
                }

                if (modelLayoutViewItem is IModelLayoutItem modelLayoutItem)
                {
                    MapModelLayoutItem(modelLayoutItem, layoutViewItemNode);
                }

                if (modelLayoutViewItem is IModelViewLayoutElement modelViewLayoutElement)
                {
                    MapModelViewLayoutElement(modelViewLayoutElement, layoutViewItemNode);
                }

                if (modelLayoutViewItem is ISupportControlAlignment modelSupportControlAlignment)
                {
                    MapSupportControlAlignment(modelSupportControlAlignment, layoutViewItemNode);
                }

                if (modelLayoutViewItem is IModelToolTip modelToolTip)
                {
                    MapModelToolTip(modelToolTip, layoutViewItemNode);
                }

                if (modelLayoutViewItem is IModelToolTipOptions modelToolTipOptions)
                {
                    MapModelToolTipOptions(modelToolTipOptions, layoutViewItemNode);
                }

                if (modelLayoutViewItem is IModelLayoutElementWithCaptionOptions modelLayoutElementWithCaptionOptions)
                {
                    MapLayoutElementWithCaptionOptions(modelLayoutElementWithCaptionOptions, layoutViewItemNode);
                }
                else if (modelLayoutViewItem.ViewItem is IModelLayoutElementWithCaptionOptions modelLayoutElementWithCaptionOptions2)
                {
                    MapLayoutElementWithCaptionOptions(modelLayoutElementWithCaptionOptions2, layoutViewItemNode);
                }

                if (modelLayoutViewItem is IModelLayoutElementWithCaption modelLayoutElementWithCaption)
                {
                    MapCaption(modelLayoutElementWithCaption, layoutViewItemNode);
                }
                else if (modelLayoutViewItem.ViewItem is IModelLayoutElementWithCaption modelLayoutElementWithCaption2)
                {
                    MapCaption(modelLayoutElementWithCaption2, layoutViewItemNode);
                }
                else if (modelLayoutViewItem.ViewItem is not null)
                {
                    if (layoutViewItemNode.Caption is not null)
                    {
                        modelLayoutViewItem.ViewItem.Caption = layoutViewItemNode.Caption;
                    }
                }

                if (layoutViewItemNode.ViewItemOptions is not null)
                {
                    if (modelLayoutViewItem.ViewItem is not null)
                    {
                        layoutViewItemNode.ViewItemOptions(modelLayoutViewItem.ViewItem);
                    }
                }

                if (layoutViewItemNode is LayoutPropertyEditorItem layoutPropertyEditorItem
                                   && layoutPropertyEditorItem.PropertyEditorOptions is not null)
                {
                    if (modelLayoutViewItem.ViewItem is IModelPropertyEditor modelPropertyEditor)
                    {
                        layoutPropertyEditorItem.PropertyEditorOptions(modelPropertyEditor);
                    }
                }

                return modelLayoutViewItem;

            }
        }
    }
}
