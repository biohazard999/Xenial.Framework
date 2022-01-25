using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace Xenial.Framework.Layouts.Items;

[XenialModelOptionsRootMapper(typeof(LayoutItemNode))]
[XenialModelOptionsMapper(typeof(LayoutGroupItem))]
[XenialModelOptionsMapper(typeof(LayoutTabbedGroupItem))]
[XenialModelOptionsMapper(typeof(LayoutTabGroupItem))]
[XenialModelOptionsMapper(typeof(LayoutStaticTextItem))]
[XenialModelOptionsMapper(typeof(LayoutStaticImageItem))]
[XenialModelOptionsMapper(typeof(LayoutPropertyEditorItem))]
[XenialModelOptionsMapper(typeof(LayoutEmptySpaceItem))]
[XenialModelOptionsMapper(typeof(LayoutDashboardViewItem))]
[XenialModelOptionsMapper(typeof(LayoutActionContainerItem))]
[XenialModelOptionsMapper(typeof(LayoutViewItem))]
internal partial class LayoutItemMapper
{
}
