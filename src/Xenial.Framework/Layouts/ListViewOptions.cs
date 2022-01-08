using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

using Xenial;

namespace Xenial.Framework.Layouts;

[XenialModelOptions(
    typeof(IModelListView), IgnoredMembers = new[]
    {
        nameof(IModelListView.Id),
        nameof(IModelListView.Index),
        nameof(IModelListView.ModelClass)
    }
)]
[XenialModelOptions(
    typeof(IModelAsync), IgnoredMembers = new[]
    {
        nameof(IModelListView.Id),
        nameof(IModelListView.Index)
    }
)]
[XenialModelOptions(typeof(IModelListViewNewItemRow))]
[XenialModelOptions(typeof(IModelListViewShowAutoFilterRow))]
[XenialModelOptions(typeof(IModelListViewShowFindPanel))]
public partial record ListViewOptions
{
}
