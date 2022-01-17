﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;

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
        OpenDevToolsSimpleAction = new SimpleAction(this, nameof(OpenDevToolsSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.Tools)
        {
            ImageName = XenialImages.Action_Xenial_DevTools,
            PaintStyle = ActionItemPaintStyle.Image,
            ToolTip = "Opens the Xenial-DevTools"
        };
        OpenDevToolsSimpleAction.Execute += OpenDevToolsSimpleAction_Execute;
    }

    private void OpenDevToolsSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        var controller = Application.MainWindow.GetController<XenialDevToolsWindowController>();
        if (controller is not null)
        {
            controller.OpenDevTools(View);
        }
    }
}
