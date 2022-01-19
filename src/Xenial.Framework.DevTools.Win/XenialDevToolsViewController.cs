using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.XtraEditors;

using Xenial.Framework.Images;

namespace Xenial.Framework.DevTools.Win;

/// <summary>
/// 
/// </summary>
public class XenialDevToolsViewController : ViewController
{
    /// <summary>
    /// 
    /// </summary>
    public SimpleAction OpenDevToolsSimpleAction { get; }
    /// <summary>
    /// 
    /// </summary>
    public XenialDevToolsViewController()
    {
        OpenDevToolsSimpleAction = new SimpleAction(this, nameof(OpenDevToolsSimpleAction), "Diagnostic")
        {
            ImageName = XenialImages.Action_Xenial_DevTools,
            PaintStyle = ActionItemPaintStyle.Image,
            ToolTip = "Opens the Xenial-DevTools",
            Shortcut = "CtrlShiftF5"
        };

        OpenDevToolsSimpleAction.CustomizeControl += OpenDevToolsSimpleAction_CustomizeControl;
        OpenDevToolsSimpleAction.Execute += OpenDevToolsSimpleAction_Execute;
    }

    private void OpenDevToolsSimpleAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
    {
        if (e.Control is SimpleButton button)
        {
            button.AutoWidthInLayoutControl = false;
            button.MinimumSize = new System.Drawing.Size(button.Height, 0);
            button.Width = button.Height;
        }
    }

    private void OpenDevToolsSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        var controller = Application.MainWindow?.GetController<XenialDevToolsWindowController>();
        if (controller is not null)
        {
            controller.OpenDevTools(View);
        }
        if (Application.MainWindow is null)
        {
            controller = Application.CreateController<XenialDevToolsWindowController>();
            controller.OpenDevTools(View);
        }
    }

}
