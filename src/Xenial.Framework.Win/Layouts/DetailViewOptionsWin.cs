using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

using Xenial.Framework.Layouts;

namespace Xenial.Framework.Win.Layouts;

[XenialModelOptions(
    typeof(IModelWinLayoutManagerOptions), IgnoredMembers = new[]
    {
        nameof(IModelWinLayoutManagerOptions.Index)
    }
)]
public partial record DetailViewOptionsWin : DetailViewOptions
{
}

[XenialModelOptionsMapper(typeof(DetailViewOptionsWin))]
internal partial class ViewOptionsMapper
{
}
