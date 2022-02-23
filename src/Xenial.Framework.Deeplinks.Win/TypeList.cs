using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Framework.Deeplinks.Win;

/// <summary>
/// 
/// </summary>
[Xenial.XenialCollectControllers]
public static partial class XenialDeeplinksTypeListWin
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="controllerTypes"></param>
    /// <returns></returns>
    public static IEnumerable<Type> UseXenialDeeplinksControllerTypesWin(this IEnumerable<Type> controllerTypes)
        => controllerTypes.Concat(ControllerTypes);
}
