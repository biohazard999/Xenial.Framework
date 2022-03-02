using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;

using System;
using System.Linq;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
public abstract class HandleDeeplinkMainWindowController : WindowController
{
    /// <summary>
    /// 
    /// </summary>
    protected HandleDeeplinkMainWindowController()
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
    /// <param name="info"></param>
    /// <param name="modelView"></param>
    /// <param name="objectKey"></param>
    /// <returns></returns>
    public bool HandleView(DeeplinkUriInfo info, IModelView modelView, string? objectKey)
    {
        if (Active)
        {
            return HandleViewCore(info, modelView, objectKey);
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="modelView"></param>
    /// <param name="objectKey"></param>
    /// <returns></returns>
    protected virtual bool HandleViewCore(DeeplinkUriInfo info!!, IModelView modelView, string? objectKey)
    {
        if (modelView is IModelDetailView modelDetailView && string.IsNullOrEmpty(objectKey))
        {
            if (info.QueryCollection.TryGetBoolean("createObject", out var createObject) && createObject)
            {
                var type = modelDetailView.ModelClass.TypeInfo.Type;
                var objectSpace = info.Application.CreateObjectSpace(type);
                var newObject = objectSpace.CreateObject(type);
                var detailView = info.Application.CreateDetailView(objectSpace, modelDetailView, true, newObject);
                var svp = new ShowViewParameters(detailView);
                info.Application.ShowViewStrategy.ShowView(svp, new(null, null));
                return true;
            }
        }

        if (modelView is IModelObjectView modelObjectView)
        {
            var shortcut = new ViewShortcut(modelObjectView.ModelClass.TypeInfo.Type, objectKey, modelView.Id);

            var objectView = info.Application.ProcessShortcut(shortcut);
            if (objectView is not null)
            {
                var svp = new ShowViewParameters(objectView);
                info.Application.ShowViewStrategy.ShowView(svp, new(null, null));
                return true;
            }
            return false;
        }

        if (modelView is IModelDashboardView modelDashboardView)
        {
            var shortcut = new ViewShortcut(null, null, modelDashboardView.Id);

            var dashboardView = info.Application.ProcessShortcut(shortcut);
            if (dashboardView is not null)
            {
                var svp = new ShowViewParameters(dashboardView);
                info.Application.ShowViewStrategy.ShowView(svp, new(null, null));
                return true;
            }

            return false;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uriInfo"></param>
    /// <param name="actionBase"></param>
    /// <returns></returns>
    public bool HandleAction(DeeplinkUriInfo uriInfo, ActionBase actionBase)
    {
        if (Active)
        {
            return HandleActionCore(uriInfo, actionBase);
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uriInfo"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    protected virtual bool HandleActionCore(DeeplinkUriInfo uriInfo, ActionBase action)
    {
        if (action is SimpleAction simpleAction)
        {
            simpleAction.DoExecute();
            return true;
        }

        if (action is SingleChoiceAction singleChoiceAction)
        {
            singleChoiceAction.DoExecute(singleChoiceAction.Items.First());
            return true;
        }

        if (action is PopupWindowShowAction popupWindowShowAction)
        {
            popupWindowShowAction.DoExecute(Application.MainWindow);
            return true;
        }

        return false;
    }

    internal void OnArgumentsHandled()
        => ArgumentsHandled?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<EventArgs>? ArgumentsHandled;

}
