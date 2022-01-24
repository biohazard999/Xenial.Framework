using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

[XenialLayoutPropertyEditorItemMapper(typeof(LayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
[XenialLayoutPropertyEditorItemMapper(typeof(StringLayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
[XenialLayoutPropertyEditorItemMapper(typeof(LookupLayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
[XenialLayoutPropertyEditorItemMapper(typeof(NumberLayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
[XenialLayoutPropertyEditorItemMapper(typeof(BooleanLayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
[XenialLayoutPropertyEditorItemMapper(typeof(LayoutActionContainerItem), typeof(IModelActionContainerViewItem))]
[XenialLayoutPropertyEditorItemMapper(typeof(LayoutStaticTextItem), typeof(IModelStaticText))]
[XenialLayoutPropertyEditorItemMapper(typeof(LayoutStaticImageItem), typeof(IModelStaticImage))]
[XenialLayoutPropertyEditorItemMapper(typeof(LayoutDashboardViewItem), typeof(IModelDashboardViewItem))]
[XenialLayoutPropertyEditorItemMapper(typeof(LayoutViewItem), typeof(IModelViewItem))]
[XenialLayoutPropertyEditorItemMapper(typeof(LayoutMemberViewItem), typeof(IModelMemberViewItem))]
internal partial class ViewItemMapper
{
    partial void MapNodeCore(LayoutViewItem from, IModelViewItem to)
    {
        if (from.WasCaptionSet)
        {
            to.Caption = from.Caption;
            if (to is IModelCommonMemberViewItem modelCommonMemberViewItem)
            {
                modelCommonMemberViewItem.Caption = from.Caption;
            }
        }
    }
}
