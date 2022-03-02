using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
public sealed class XafDeeplinkDispatcher
{
    /// <summary>
    /// 
    /// </summary>
    public XafApplication Application { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public XafDeeplinkDispatcher(XafApplication application)
        => Application = application;

    private static Dictionary<string, Func<DeeplinkUriInfo, bool>> ProtocolHandlers { get; } = new Dictionary<string, Func<DeeplinkUriInfo, bool>>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handler"></param>
    public static void RegisterProtocolHandler(string verb, Func<DeeplinkUriInfo, bool> handler)
        => ProtocolHandlers[verb] = handler;

    static XafDeeplinkDispatcher()
    {
        RegisterProtocolHandler(DefaultDeeplinkVerbs.View, HandleViewProtocol);
        RegisterProtocolHandler(DefaultDeeplinkVerbs.Action, HandleActionProtocol);
    }

    private static bool HandleViewProtocol(DeeplinkUriInfo info)
    {
        static (string viewId, string? objectKey) ExtractViewInfo(DeeplinkUriInfo info)
        {
            if (info.Route.Contains('/'
#if NET5_0_OR_GREATER
            , StringComparison.Ordinal
#endif
        ))
            {
                var splittedRoute = info.Route.Split('/');
                if (splittedRoute.Length > 1)
                {
                    return (splittedRoute[0], splittedRoute[1]);
                }
                return (splittedRoute[0], null);
            }
            return (info.Route, null);
        }

        var (viewId, objectKey) = ExtractViewInfo(info);

        var modelView = info.Application.FindModelView(viewId);

        if (modelView is null)
        {
            throw new UserFriendlyException(
                DevExpress.ExpressApp.Localization.UserVisibleExceptionId.TheFollowingErrorOccurred,
                new ViewNotFoundException(viewId)
            );
        }

        if (info.Application.MainWindow is not null)
        {
            var controller = info.Application.MainWindow.GetController<HandleDeeplinkMainWindowController>();
            if (controller is not null)
            {
                if (controller.HandleView(info, modelView, objectKey))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HandleActionProtocol(DeeplinkUriInfo info)
    {
        static string ExtractActionInfo(DeeplinkUriInfo info)
        {
            if (info.Route.Contains('/'
#if NET5_0_OR_GREATER
            , StringComparison.Ordinal
#endif
        ))
            {
                return info.Route.Split('/')[0];
            }
            return info.Route;
        }

        var actionId = ExtractActionInfo(info);

        static ActionBase? FindAction(string actionId, XafApplication application)
        {
            if (application.MainWindow is not null)
            {
                foreach (var controller in application.MainWindow.Controllers)
                {
                    foreach (var action in controller.Actions)
                    {
                        if (action.Id == actionId)
                        {
                            return action;
                        }
                    }
                }
            }

            return null;
        }

        var action = FindAction(actionId, info.Application);

        if (info.Application.MainWindow is not null && action is not null)
        {
            var controller = info.Application.MainWindow.GetController<HandleDeeplinkMainWindowController>();
            if (controller is not null)
            {
                if (controller.HandleAction(info, action))
                {
                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="arguments"></param>
    public void HandleArguments(IList<string> arguments)
    {
        _ = arguments ?? throw new ArgumentNullException(nameof(arguments));

        if (Application.Model.Options is IModelOptionsDeeplinkProtocols modelOptionsDeeplinkProtocols)
        {
            if (!modelOptionsDeeplinkProtocols.DeeplinkProtocols.EnableProtocols)
            {
                return;
            }
        }
        else
        {
            return;
        }

        if (arguments.Count == 0)
        {
            return;
        }

        if (arguments.Count == 1)
        {
            var firstArgument = arguments[0];
            if (Uri.TryCreate(firstArgument, UriKind.Absolute, out var parsedUri))
            {
                if (HandleUri(parsedUri))
                {
                    var controller = Application.MainWindow?.GetController<HandleDeeplinkMainWindowController>();
                    controller?.OnArgumentsHandled();
                    return;
                }
            }
        }
    }

    private bool HandleUri(Uri uri)
    {
        var protocol = Protocols.FirstOrDefault(m => m.ProtocolName == uri.Scheme);
        if (protocol is not null)
        {
            if (HandleProtocol(protocol, uri))
            {
                return true;
            }
        }

        return false;
    }

    private bool HandleProtocol(IModelDeeplinkProtocol protocol, Uri uri)
    {
        var info = new DeeplinkUriInfo(Application, protocol, uri);
        if (ProtocolHandlers.ContainsKey(info.Verb))
        {
            return ProtocolHandlers[info.Verb](info);
        }
        return false;
    }

    private IModelDeeplinkProtocols Protocols => ((IModelOptionsDeeplinkProtocols)Application.Model.Options).DeeplinkProtocols;
}
