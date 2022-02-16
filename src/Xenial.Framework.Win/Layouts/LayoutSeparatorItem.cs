﻿using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout separator item. </summary>
[XenialCheckLicense]
[XenialModelOptions(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelSeparator))]
public partial record LayoutSeparatorItem : LayoutViewItemNode
{
}