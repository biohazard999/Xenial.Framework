using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

namespace Xenial.Framework.Deeplinks.Win;

/// <summary>
/// 
/// </summary>
public sealed class XenialDeeplinksWindowsFormsModule : XenialModuleBase
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultValue(true)]
    public bool AutoInstallDeeplinkProtocols { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<Type> GetDeclaredControllerTypes()
        => base.GetDeclaredControllerTypes()
        .UseXenialDeeplinksControllerTypesWin();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public override void Setup(XafApplication application)
    {
        base.Setup(application);

        if (AutoInstallDeeplinkProtocols)
        {
            application.InstallXenialDeeplinkProtocols();
        }
    }
}
