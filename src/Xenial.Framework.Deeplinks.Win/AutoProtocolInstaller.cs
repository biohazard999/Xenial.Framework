using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.Deeplinks.Model;
using Xenial.Framework.Deeplinks.Win.Helpers;

namespace DevExpress.ExpressApp;


/// <summary>
/// 
/// </summary>
/// <param name="Protocol"></param>
/// <param name="ResolveExecutable"></param>
public record ProtocolExecutableResolver(string Protocol, Func<string> ResolveExecutable);

/// <summary>
/// 
/// </summary>
public static class AutoProtocolInstaller
{
    static AutoProtocolInstaller() => ResolveDefaultExecutable(() =>
        System.Windows.Forms.Application.ExecutablePath
    );

    private static List<Func<string>> DefaultExecutableResolvers { get; } = new();
    private static List<ProtocolExecutableResolver> ExecutableResolvers { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resolveExecutable"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ResolveDefaultExecutable(Func<string> resolveExecutable)
    {
        _ = resolveExecutable ?? throw new ArgumentNullException(nameof(resolveExecutable));
        DefaultExecutableResolvers.Insert(0, resolveExecutable);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="protocolName"></param>
    /// <param name="resolveExecutable"></param>
    public static void ResolveProtocolExecutable(string protocolName, Func<string> resolveExecutable)
    {
        if (string.IsNullOrEmpty(protocolName))
        {
            throw new ArgumentNullException(nameof(protocolName));
        }
        _ = resolveExecutable ?? throw new ArgumentNullException(nameof(resolveExecutable));

        ExecutableResolvers.Insert(0, new(protocolName, resolveExecutable));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resolver"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ResolveProtocolExecutable(ProtocolExecutableResolver resolver)
    {
        _ = resolver ?? throw new ArgumentNullException(nameof(resolver));

        ExecutableResolvers.Insert(0, resolver);
    }

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

        static void Application_SetupComplete(object? sender, EventArgs e)
        {
            if (sender is XafApplication app)
            {
                app.SetupComplete -= Application_SetupComplete;

                if (app.Model.Options is IModelOptionsDeeplinkProtocols modelOptionsDeeplinkProtocols && modelOptionsDeeplinkProtocols.DeeplinkProtocols is not null)
                {
                    var exePath = DefaultExecutableResolvers.First()();
                    foreach (var deepLinkProtocol in modelOptionsDeeplinkProtocols.DeeplinkProtocols)
                    {
                        var protocolResolver = ExecutableResolvers.FirstOrDefault(m => m.Protocol.Equals(deepLinkProtocol.ProtocolName, StringComparison.OrdinalIgnoreCase));

                        if (protocolResolver is not null)
                        {
                            exePath = protocolResolver.ResolveExecutable();
                        }

                        var protocol = new Protocol(exePath, deepLinkProtocol.ProtocolName, deepLinkProtocol.ProtocolDescription);
                        ProtocolInstaller.UnRegisterProtocol(protocol);
                        ProtocolInstaller.RegisterProtocol(protocol);
                    }
                }
            }
        }

        static void Application_Disposed(object? sender, EventArgs e)
        {
            if (sender is XafApplication app)
            {
                app.Disposed -= Application_Disposed;
                app.SetupComplete -= Application_SetupComplete;
            }
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

        static void Application_SetupComplete(object? sender, EventArgs e)
        {
            if (sender is XafApplication app)
            {
                app.SetupComplete -= Application_SetupComplete;

                if (app.Model.Options is IModelOptionsDeeplinkProtocols modelOptionsDeeplinkProtocols && modelOptionsDeeplinkProtocols.DeeplinkProtocols is not null)
                {
                    var exePath = DefaultExecutableResolvers.First()();

                    foreach (var deepLinkProtocol in modelOptionsDeeplinkProtocols.DeeplinkProtocols)
                    {
                        var protocol = new Protocol(exePath, deepLinkProtocol.ProtocolName, deepLinkProtocol.ProtocolDescription);
                        ProtocolInstaller.UnRegisterProtocol(protocol);
                    }
                }
            }
        }

        static void Application_Disposed(object? sender, EventArgs e)
        {
            if (sender is XafApplication app)
            {
                app.Disposed -= Application_Disposed;
                app.SetupComplete -= Application_SetupComplete;
            }
        }
    }
}
