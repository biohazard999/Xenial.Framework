using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

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
public partial record ListViewOptions
{
}
