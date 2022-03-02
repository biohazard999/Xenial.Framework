using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

using System;
using System.Linq;

using Xenial.Framework.Deeplinks.Model;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
public sealed class HandleDeeplinkViewEventArgs : EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="modelView"></param>
    /// <param name="objectKey"></param>
    public HandleDeeplinkViewEventArgs(DeeplinkUriInfo info, IModelView modelView, string? objectKey)
        => (Info, ModelView, ObjectKey) = (info, modelView, objectKey);

    /// <summary>
    /// 
    /// </summary>
    public DeeplinkUriInfo Info { get; }

    /// <summary>
    /// 
    /// </summary>
    public IModelView ModelView { get; }
    /// <summary>
    /// 
    /// </summary>
    public string? ObjectKey { get; }
    /// <summary>
    /// 
    /// </summary>
    public bool Handled { get; set; }
}

/// <summary>
/// 
/// </summary>
public sealed class HandleDeeplinkActionEventArgs : EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="action"></param>
    public HandleDeeplinkActionEventArgs(DeeplinkUriInfo info, ActionBase action)
        => (Info, Action) = (info, action);

    /// <summary>
    /// 
    /// </summary>
    public DeeplinkUriInfo Info { get; }

    /// <summary>
    /// 
    /// </summary>
    public ActionBase Action { get; }
    /// <summary>
    /// 
    /// </summary>
    public bool Handled { get; set; }
}

/// <summary>
/// 
/// </summary>
public abstract class HandleDeeplinkMainWindowController : WindowController
{
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<ObjectCreatedEventArgs>? ObjectCreated;

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<HandleDeeplinkViewEventArgs>? CustomHandleView;

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<HandleDeeplinkActionEventArgs>? CustomHandleAction;

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
            var args = new HandleDeeplinkViewEventArgs(info, modelView, objectKey);
            CustomHandleView?.Invoke(this, args);
            if (args.Handled)
            {
                return true;
            }

            return HandleViewCore(info, modelView, objectKey);
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newObject"></param>
    /// <param name="objectSpace"></param>
    protected virtual void OnObjectCreated(object newObject, IObjectSpace objectSpace)
        => ObjectCreated?.Invoke(this, new ObjectCreatedEventArgs(newObject, objectSpace));

    private void RaiseObjectCreated(object newObject, IObjectSpace objectSpace)
        => OnObjectCreated(newObject, objectSpace);

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
                RaiseObjectCreated(newObject, objectSpace);
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
    /// <param name="info"></param>
    /// <param name="actionBase"></param>
    /// <returns></returns>
    public bool HandleAction(DeeplinkUriInfo info, ActionBase actionBase)
    {
        if (Active)
        {
            var args = new HandleDeeplinkActionEventArgs(info, actionBase);
            CustomHandleAction?.Invoke(this, args);
            if (args.Handled)
            {
                return true;
            }

            return HandleActionCore(info, actionBase);
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
