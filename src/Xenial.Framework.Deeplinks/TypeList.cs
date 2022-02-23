using System;
using System.Collections.Generic;
using System.Linq;

using Xenial;

namespace DevExpress.ExpressApp;

/// <summary>
/// 
/// </summary>
[XenialCollectControllers]
public static partial class XenialDeeplinksTypeList
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="controllerTypes"></param>
    /// <returns></returns>
    public static IEnumerable<Type> UseXenialDeeplinksControllerTypes(this IEnumerable<Type> controllerTypes)
        => controllerTypes.Concat(ControllerTypes);
}
