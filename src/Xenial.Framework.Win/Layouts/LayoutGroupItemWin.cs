using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Win.SystemModule;

using Xenial.Framework.Layouts.Items;

namespace Xenial.Framework.Win.Layouts;

/// <summary>
/// 
/// </summary>
[XenialModelOptions(
    typeof(IModelWinLayoutGroup), IgnoredMembers = new[]
    {
        nameof(IModelWinLayoutGroup.Index)
    }
)]
public partial record LayoutGroupItemWin : LayoutGroupItem
{
}
