using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

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

}
