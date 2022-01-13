using System;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.PubTernal;

#pragma warning disable CA1508 // is never null -> Check later //TODO: CHECK LATER

namespace Xenial.Framework.Model.GeneratorUpdaters;

internal static class ModelDetailViewLayoutNodesGeneratorUpdaterMappers
{
    internal static void MapCaption(
        IModelLayoutElementWithCaption modelLayoutElementWithCaption,
        ILayoutElementWithCaption layoutViewItemNode
    )
    {
        if (layoutViewItemNode.Caption is not null)
        {
            modelLayoutElementWithCaption.Caption =
                layoutViewItemNode.Caption ?? modelLayoutElementWithCaption.Caption;
        }
    }

    internal static void MapModelViewLayoutElement(
        IModelViewLayoutElement modelModelViewLayoutElement,
        LayoutItemNode layoutViewItemNode
    )
    {
        if (layoutViewItemNode.Id is not null)
        {
            modelModelViewLayoutElement.Id =
                layoutViewItemNode.Id ?? modelModelViewLayoutElement.Id;
        }

        if (layoutViewItemNode.RelativeSize is not null)
        {
            modelModelViewLayoutElement.RelativeSize =
                layoutViewItemNode.RelativeSize ?? modelModelViewLayoutElement.RelativeSize;
        }
    }

    internal static void MapLayoutElementWithCaptionOptions(
        IModelLayoutElementWithCaptionOptions modelLayoutElementWithCaption,
        ILayoutElementWithCaptionOptions layoutViewItemNode
    )
    {
        if (layoutViewItemNode.ShowCaption is not null)
        {
            modelLayoutElementWithCaption.ShowCaption =
                layoutViewItemNode.ShowCaption ?? modelLayoutElementWithCaption.ShowCaption;
        }

        if (layoutViewItemNode.CaptionLocation is not null)
        {
            modelLayoutElementWithCaption.CaptionLocation =
                layoutViewItemNode.CaptionLocation ?? modelLayoutElementWithCaption.CaptionLocation;
        }

        if (layoutViewItemNode.CaptionHorizontalAlignment is not null)
        {
            modelLayoutElementWithCaption.CaptionHorizontalAlignment =
                layoutViewItemNode.CaptionHorizontalAlignment ?? modelLayoutElementWithCaption.CaptionHorizontalAlignment;
        }

        if (layoutViewItemNode.CaptionVerticalAlignment is not null)
        {
            modelLayoutElementWithCaption.CaptionVerticalAlignment =
                layoutViewItemNode.CaptionVerticalAlignment ?? modelLayoutElementWithCaption.CaptionVerticalAlignment;
        }

        if (layoutViewItemNode.CaptionWordWrap is not null)
        {
            modelLayoutElementWithCaption.CaptionWordWrap =
                layoutViewItemNode.CaptionWordWrap ?? modelLayoutElementWithCaption.CaptionWordWrap;
        }
    }

    internal static void MapModelNode(
        IModelNode genericModelNode,
        LayoutItemNode genericLayoutItemNode
    )
    {
        if (genericLayoutItemNode.Index is not null)
        {
            genericModelNode.Index =
                genericLayoutItemNode.Index ?? genericModelNode.Index;
        }
    }

    internal static void MapSupportControlAlignment(
        ISupportControlAlignment modelSupportControlAlignment,
        ILayoutItemNodeWithAlign layoutItemNodeWithAlign
    )
    {
        if (layoutItemNodeWithAlign.HorizontalAlign is not null)
        {
            modelSupportControlAlignment.HorizontalAlign =
                layoutItemNodeWithAlign.HorizontalAlign ?? modelSupportControlAlignment.HorizontalAlign;
        }

        if (layoutItemNodeWithAlign.VerticalAlign is not null)
        {
            modelSupportControlAlignment.VerticalAlign =
                layoutItemNodeWithAlign.VerticalAlign ?? modelSupportControlAlignment.VerticalAlign;
        }
    }

    internal static void MapModelLayoutItem(
        IModelLayoutItem modelLayoutItem,
        LayoutItemLeaf layoutItemLeaf
    )
    {
        if (layoutItemLeaf.SizeConstraintsType is not null)
        {
            modelLayoutItem.SizeConstraintsType =
                layoutItemLeaf.SizeConstraintsType ?? modelLayoutItem.SizeConstraintsType;
        }

        if (layoutItemLeaf.MinSize is not null)
        {
            modelLayoutItem.MinSize =
                layoutItemLeaf.MinSize ?? modelLayoutItem.MinSize;
        }

        if (layoutItemLeaf.MaxSize is not null)
        {
            modelLayoutItem.MaxSize =
                layoutItemLeaf.MaxSize ?? modelLayoutItem.MaxSize;
        }
    }

    internal static void MapModelToolTip(
        IModelToolTip modelToolTip,
        ILayoutToolTip layoutViewItemNode
    )
    {
        if (layoutViewItemNode.ToolTip is not null)
        {
            modelToolTip.ToolTip =
                layoutViewItemNode.ToolTip ?? modelToolTip.ToolTip;
        }
    }

    internal static void MapModelToolTipOptions(
        IModelToolTipOptions modelToolTipOptions,
        ILayoutToolTipOptions layoutViewItemNode
    )
    {
        if (layoutViewItemNode.ToolTipTitle is not null)
        {
            modelToolTipOptions.ToolTipTitle =
                layoutViewItemNode.ToolTipTitle ?? modelToolTipOptions.ToolTipTitle;
        }

        if (layoutViewItemNode.ToolTipIconType is not null)
        {
            modelToolTipOptions.ToolTipIconType =
                layoutViewItemNode.ToolTipIconType ?? modelToolTipOptions.ToolTipIconType;
        }
    }

    internal static void MapLayoutGroup(
        IModelLayoutGroup modelLayoutGroup,
        ILayoutGroupItem groupItemNode
    )
    {
        modelLayoutGroup.Direction = groupItemNode.Direction;

        if (groupItemNode.ImageName is not null)
        {
            modelLayoutGroup.ImageName =
                groupItemNode.ImageName ?? modelLayoutGroup.ImageName;
        }

        if (groupItemNode.IsCollapsibleGroup is not null)
        {
            modelLayoutGroup.IsCollapsibleGroup =
                groupItemNode.IsCollapsibleGroup ?? modelLayoutGroup.IsCollapsibleGroup;
        }
    }

    internal static void MapTabbedLayoutGroup(
        IModelTabbedGroup modelTabbedGroup,
        LayoutTabbedGroupItem tabbedGroupItemNode
    )
    {
        modelTabbedGroup.Direction = tabbedGroupItemNode.Direction;

        if (tabbedGroupItemNode.MultiLine is not null)
        {
            modelTabbedGroup.MultiLine =
                tabbedGroupItemNode.MultiLine ?? modelTabbedGroup.MultiLine;
        }
    }
}
