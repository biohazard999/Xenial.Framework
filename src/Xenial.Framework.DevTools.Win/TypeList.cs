using System;
using System.Collections.Generic;
using System.Linq;

using Xenial;

namespace DevExpress.ExpressApp;

/// <summary>
/// 
/// </summary>
[XenialCollectControllers]
public static partial class XenialDevToolsWindowsFormsTypeList
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public static IEnumerable<Type> UseXenialDevToolsWindowsFormsControllers(this IEnumerable<Type> types)
        => types.Concat(ControllerTypes);
}
