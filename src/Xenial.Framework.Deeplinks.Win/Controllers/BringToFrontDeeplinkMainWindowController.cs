using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

namespace Xenial.Framework.Deeplinks.Win;

/// <summary>
/// 
/// </summary>
public sealed class BringToFrontDeeplinkMainWindowController : WindowController
{
    /// <summary>
    /// 
    /// </summary>
    public BringToFrontDeeplinkMainWindowController()
        => TargetWindowType = WindowType.Main;

    /// <summary>
    /// 
    /// </summary>
    protected override void OnActivated()
    {
        base.OnActivated();
        var controller = Frame.GetController<HandleDeeplinkMainWindowController>();
        if (controller is not null)
        {
            controller.ArgumentsHandled -= Controller_ArgumentsHandled;
            controller.ArgumentsHandled += Controller_ArgumentsHandled;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        var controller = Frame.GetController<HandleDeeplinkMainWindowController>();
        if (controller is not null)
        {
            controller.ArgumentsHandled -= Controller_ArgumentsHandled;
        }
    }

    private void Controller_ArgumentsHandled(object? sender, EventArgs e)
    {
        if (Window is WinWindow winWindow && winWindow.Form is System.Windows.Forms.Form form)
        {
            form.BringToFront();
        }
    }
}
