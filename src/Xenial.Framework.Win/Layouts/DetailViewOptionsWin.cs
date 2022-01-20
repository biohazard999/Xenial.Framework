using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Win.SystemModule;

namespace Xenial.Framework.Layouts;

/// <summary>
/// 
/// </summary>
[XenialModelOptions(
    typeof(IModelWinLayoutManagerOptions), IgnoredMembers = new[]
    {
        nameof(IModelWinLayoutManagerOptions.Index)
    }
)]
public partial record DetailViewOptionsWin : DetailViewOptions, IDetailViewOptionsExtension
{
}

[XenialModelOptionsMapper(typeof(DetailViewOptionsWin))]
internal partial class ViewOptionsMapper
{
}
