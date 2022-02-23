using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks.Controllers;

/// <summary>
/// 
/// </summary>
public sealed class HandleDeeplinkMainWindowController : WindowController
{
    static HandleDeeplinkMainWindowController() =>
        RegisterProtocolHandler(DefaultDeeplinkVerbs.View, HandleViewProtocol);

    private static bool HandleViewProtocol(DeeplinkUriInfo info)
    {
        if (info.Route.Contains('/'
#if NET5_0_OR_GREATER
, StringComparison.Ordinal
#endif
        ))
        {
            var routeSegments = info.Route.Split("/");
            var viewId = routeSegments[0];

            var modelView = info.Application.FindModelView(viewId);

            if (modelView is null)
            {
                throw new UserFriendlyException(
                    DevExpress.ExpressApp.Localization.UserVisibleExceptionId.TheFollowingErrorOccurred,
                    new ViewNotFoundException(viewId)
                );
            }

            if (modelView is IModelDetailView modelDetailView)
            {
                var objectSpace = info.Application.CreateObjectSpace(modelDetailView.ModelClass.TypeInfo.Type);

                object? FindOrCreateObject()
                {
                    if (routeSegments.Length > 1)
                    {
                        var keyAsString = routeSegments[1];
                        var keyType = objectSpace.GetKeyPropertyType(modelDetailView.ModelClass.TypeInfo.Type);
                        if (keyType == typeof(Guid))
                        {
                            if (Guid.TryParse(keyAsString, out var guidKey))
                            {
                                return objectSpace.GetObjectByKey(modelDetailView.ModelClass.TypeInfo.Type, guidKey);
                            }
                        }

                        if (keyType == typeof(int))
                        {
                            if (int.TryParse(keyAsString, out var intKey))
                            {
                                return objectSpace.GetObjectByKey(modelDetailView.ModelClass.TypeInfo.Type, intKey);
                            }
                        }

                        var key = keyAsString;
                        return objectSpace.GetObjectByKey(modelDetailView.ModelClass.TypeInfo.Type, key);
                    }

                    return objectSpace.CreateObject(modelDetailView.ModelClass.TypeInfo.Type);
                }

                var obj = FindOrCreateObject();

                var detailView = obj is null
                    ? info.Application.CreateDetailView(objectSpace, modelDetailView, true)
                    : info.Application.CreateDetailView(objectSpace, obj, true);

                var svp = new ShowViewParameters(detailView);
                info.Application.ShowViewStrategy.ShowView(svp, new(null, null));
                return true;
            }

            if (modelView is IModelListView modelListView)
            {
                var listView = info.Application.CreateListView(modelListView.ModelClass.TypeInfo.Type, true);
                var svp = new ShowViewParameters(listView);
                info.Application.ShowViewStrategy.ShowView(svp, new(null, null));
                return true;
            }

            if (modelView is IModelDashboardView modelDashboardView)
            {
                var objectSpace = info.Application.CreateObjectSpace();
                var listView = info.Application.CreateDashboardView(objectSpace, modelDashboardView.Id, true);
                var svp = new ShowViewParameters(listView);
                info.Application.ShowViewStrategy.ShowView(svp, new(null, null));
                return true;
            }
        }

        return false;
    }

    private IModelDeeplinkProtocols Protocols => ((IModelOptionsDeeplinkProtocols)Application.Model.Options).DeeplinkProtocols;

    private static Dictionary<string, Func<DeeplinkUriInfo, bool>> ProtocolHandlers { get; } = new Dictionary<string, Func<DeeplinkUriInfo, bool>>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handler"></param>
    public static void RegisterProtocolHandler(string verb, Func<DeeplinkUriInfo, bool> handler)
        => ProtocolHandlers[verb] = handler;

    /// <summary>
    /// 
    /// </summary>
    public HandleDeeplinkMainWindowController()
        => TargetWindowType = WindowType.Main;

    /// <summary>
    /// 
    /// </summary>
    protected override void OnActivated()
    {
        base.OnActivated();
        if (Application.Model.Options is IModelOptionsDeeplinkProtocols modelOptionsDeeplinkProtocols)
        {
            Active[nameof(modelOptionsDeeplinkProtocols.DeeplinkProtocols.EnableProtocols)] = modelOptionsDeeplinkProtocols.DeeplinkProtocols.EnableProtocols;
            return;
        }

        Active[nameof(modelOptionsDeeplinkProtocols.DeeplinkProtocols.EnableProtocols)] = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arguments"></param>
    public void HandleArguments(IList<string> arguments)
    {
        _ = arguments ?? throw new ArgumentNullException(nameof(arguments));

        if (!Active || arguments.Count == 0)
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
}
