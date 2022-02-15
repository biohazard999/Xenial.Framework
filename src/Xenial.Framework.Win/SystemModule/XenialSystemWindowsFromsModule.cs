using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Framework.Win.SystemModule;

/// <summary>
/// 
/// </summary>
public sealed class XenialSystemWindowsFromsModule : XenialModuleBase
{
    static XenialSystemWindowsFromsModule() =>
        XenialWindowsFormsModuleInitializer.Initialize();
}
