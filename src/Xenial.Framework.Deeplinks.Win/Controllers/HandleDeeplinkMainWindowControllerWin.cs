
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win;

using System;
using System.Linq;

namespace Xenial.Framework.Deeplinks.Win;

/// <summary>
/// 
/// </summary>
public sealed class HandleDeeplinkMainWindowControllerWin : HandleDeeplinkMainWindowController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uriInfo"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    protected override bool HandleActionCore(DeeplinkUriInfo uriInfo, ActionBase action)
    {
        if (action is PopupWindowShowAction popupWindowShowAction)
        {
            using var helper = new PopupWindowShowActionHelper(popupWindowShowAction);
            helper.ShowPopupWindow(true);
            return true;
        }

        return base.HandleActionCore(uriInfo, action);
    }
}
