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

/// <summary>
/// 
/// </summary>
public static class WinDetailViewOptionsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DetailViewOptionsExtensions WindowsForms(this DetailViewOptionsExtensions list, DetailViewOptionsWin options)
    {
        _ = list ?? throw new ArgumentNullException(nameof(list));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        list.Add(options);

        return list;
    }
}
