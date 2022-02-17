using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExpress.ExpressApp;

/// <summary>
/// 
/// </summary>
[Xenial.XenialCollectExportedTypes]
public static partial class XenialDevToolsTypeList
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public static IEnumerable<Type> UseXenialDevToolsExportedTypes(this IEnumerable<Type> types)
        => types.Concat(ExportedTypes);
}
