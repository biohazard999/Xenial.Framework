using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExpress.ExpressApp;

/// <summary>
/// Class XenialControllerTypeListExtensionWindowsForms.
/// </summary>
[Xenial.XenialCollectControllers]
public static partial class XenialControllerTypeListExtensionWindowsForms
{
    /// <summary>   Uses all Controllers of the XenialSystemWindowsFormsModule. </summary>
    ///
    /// <param name="types">    . </param>
    ///
    /// <returns>
    /// An enumerator that allows foreach to be used to process use xenial windows forms controllers
    /// in this collection.
    /// </returns>
    public static IEnumerable<Type> UseXenialWindowsFormsControllers(this IEnumerable<Type> types)
        => types.Concat(ControllerTypeList.ControllerTypes);
}

