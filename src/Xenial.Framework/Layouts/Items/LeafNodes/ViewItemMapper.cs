using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

[XenialModelOptionsRootMapper(typeof(LayoutItemNode))]
[XenialModelOptionsMapper(typeof(LayoutPropertyEditorItem))]
[XenialModelOptionsMapper(typeof(StringLayoutPropertyEditorItem))]
[XenialModelOptionsMapper(typeof(LookupLayoutPropertyEditorItem))]
[XenialModelOptionsMapper(typeof(NumberLayoutPropertyEditorItem))]
[XenialModelOptionsMapper(typeof(BooleanLayoutPropertyEditorItem))]
[XenialModelOptionsMapper(typeof(LayoutActionContainerItem))]
[XenialModelOptionsMapper(typeof(LayoutStaticTextItem))]
[XenialModelOptionsMapper(typeof(LayoutStaticImageItem))]
[XenialModelOptionsMapper(typeof(LayoutDashboardViewItem))]
[XenialModelOptionsMapper(typeof(LayoutViewItem))]
[XenialModelOptionsMapper(typeof(LayoutMemberViewItem))]
internal partial class ViewItemMapper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "By Convention")]
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
