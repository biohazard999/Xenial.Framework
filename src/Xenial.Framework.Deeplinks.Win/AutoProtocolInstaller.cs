using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using Xenial.Framework.Deeplinks.Model;
using Xenial.Framework.Deeplinks.Win.Helpers;

namespace DevExpress.ExpressApp;

/// <summary>
/// 
/// </summary>
public static class AutoProtocolInstaller
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void InstallXenialDeeplinkProtocols(this XafApplication application)
    {
        _ = application ?? throw new ArgumentNullException(nameof(application));

        application.SetupComplete -= Application_SetupComplete;
        application.SetupComplete += Application_SetupComplete;
        application.Disposed -= Application_Disposed;
        application.Disposed += Application_Disposed;

        void Application_SetupComplete(object sender, EventArgs e)
        {
            application.SetupComplete -= Application_SetupComplete;

            if (application.Model.Options is IModelOptionsDeeplinkProtocols modelOptionsDeeplinkProtocols && modelOptionsDeeplinkProtocols.DeeplinkProtocols is not null)
            {
#if NET6_0_OR_GREATER
                var exePath = Environment.ProcessPath;
#else
                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
#endif
                foreach (var deepLinkProtocol in modelOptionsDeeplinkProtocols.DeeplinkProtocols)
                {
                    var protocol = new Protocol(exePath, deepLinkProtocol.ProtocolName, deepLinkProtocol.ProtocolDescription);
                    ProtocolInstaller.UnRegisterProtocol(protocol);
                    ProtocolInstaller.RegisterProtocol(protocol);
                }
            }
        }

        void Application_Disposed(object sender, EventArgs e)
        {
            application.Disposed -= Application_Disposed;
            application.SetupComplete -= Application_SetupComplete;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void UninstallXenialDeeplinkProtocols(this XafApplication application)
    {
        _ = application ?? throw new ArgumentNullException(nameof(application));

        application.SetupComplete -= Application_SetupComplete;
        application.SetupComplete += Application_SetupComplete;
        application.Disposed -= Application_Disposed;
        application.Disposed += Application_Disposed;

        void Application_SetupComplete(object sender, EventArgs e)
        {
            application.SetupComplete -= Application_SetupComplete;

            if (application.Model.Options is IModelOptionsDeeplinkProtocols modelOptionsDeeplinkProtocols && modelOptionsDeeplinkProtocols.DeeplinkProtocols is not null)
            {
                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                foreach (var deepLinkProtocol in modelOptionsDeeplinkProtocols.DeeplinkProtocols)
                {
                    var protocol = new Protocol(exePath, deepLinkProtocol.ProtocolName, deepLinkProtocol.ProtocolDescription);
                    ProtocolInstaller.UnRegisterProtocol(protocol);
                }
            }
        }

        void Application_Disposed(object sender, EventArgs e)
        {
            application.Disposed -= Application_Disposed;
            application.SetupComplete -= Application_SetupComplete;
        }
    }
}
