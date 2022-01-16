using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates.Bars;
using DevExpress.XtraBars;

using Xenial.Framework.Images;

namespace Xenial.Framework.DevTools.Win;

public class XenialDevToolsController : DialogController
{
    /// <summary>
    /// 
    /// </summary>
    public SimpleAction AlwaysOnTopSimpleAction { get; }
    public SimpleAction OpacitySimpleAction { get; }
    /// <summary>
    /// 
    /// </summary>
    public XenialDevToolsController()
    {
        AlwaysOnTopSimpleAction = new SimpleAction(this, nameof(AlwaysOnTopSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.ObjectsCreation)
        {
            ImageName = XenialImages.Action_Xenial_AlwaysOnTop,
            PaintStyle = ActionItemPaintStyle.Image,
            ToolTip = "Controls if the Xenial-DevTools window is displayed over the current application"
        };
        OpacitySimpleAction = new SimpleAction(this, nameof(OpacitySimpleAction), DevExpress.Persistent.Base.PredefinedCategory.ObjectsCreation)
        {
            ImageName = XenialImages.Action_Xenial_Opacity,
            PaintStyle = ActionItemPaintStyle.Image,
            ToolTip = "Controls if the Xenial-DevTools window is displayed opaque"
        };
        AlwaysOnTopSimpleAction.Execute += AlwaysOnTopSimpleAction_Execute;
        OpacitySimpleAction.Execute += OpacitySimpleAction_Execute;

        AlwaysOnTopSimpleAction.CustomizeControl += AlwaysOnTopSimpleAction_CustomizeControl;
        OpacitySimpleAction.CustomizeControl += AlwaysOnTopSimpleAction_CustomizeControl;
    }

    private void AlwaysOnTopSimpleAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
    {
        if (e.Control is BarButtonItem button)
        {
            button.ButtonStyle = BarButtonStyle.Check;
            button.Down = true;
        }
    }

    private System.Windows.Forms.Form? ownerForm;

    private void AlwaysOnTopSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        if (Frame is WinWindow winWindow && winWindow.Form is not null)
        {
            if (winWindow.Form.Owner is null)
            {
                winWindow.Form.Owner = ownerForm;
            }

            winWindow.Form.TopMost = !winWindow.Form.TopMost;
            ownerForm = winWindow.Form.Owner;

            if (!winWindow.Form.TopMost)
            {
                winWindow.Form.Owner = null;
            }
        }
    }

    private bool shouldBeTransparent;
    private bool opacitySimpleActionDown = true;
    private bool Transparent => shouldBeTransparent && opacitySimpleActionDown;

    private void OpacitySimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        opacitySimpleActionDown = !opacitySimpleActionDown;
        shouldBeTransparent = !shouldBeTransparent;
        MouseDown(null, EventArgs.Empty);
    }

    private void MouseDown(object? sender, EventArgs e)
    {
        if (template is not null)
        {
            if (!Transparent)
            {
                template.Opacity = 1;
            }
            else
            {
                if (shouldBeTransparent)
                {
                    template.Opacity = 1;
                }
                else
                {
                    template.Opacity = 0.75;
                }
            }
        }
    }

    private void MouseUp(object? sender, EventArgs e)
    {
        if (template is not null)
        {
            if (!Transparent)
            {
                template.Opacity = 1;
            }
            else
            {
                if (shouldBeTransparent)
                {
                    template.Opacity = 0.75;
                }
                else
                {
                    template.Opacity = 1;
                }
            }
        }
    }

    private System.Windows.Forms.Form? template;

    private void Attach()
    {
        Detach();
        if (template is not null)
        {
            template.Activated += MouseDown;
            template.GotFocus += MouseDown;
            template.MouseDown += MouseDown;

            template.Deactivate += MouseUp;
            template.MouseUp += MouseUp;
            template.LostFocus += MouseUp;

            template.FormClosed += TemplateDisposed;
            template.Disposed += TemplateDisposed;

            if (Application.MainWindow is WinWindow winWindow)
            {
                template.Owner = winWindow.Form;
                template.TopMost = true;
            }
        }
    }
    private void Detach()
    {
        if (template is not null)
        {
            template.Activated -= MouseDown;
            template.GotFocus -= MouseDown;
            template.MouseDown -= MouseDown;

            template.Deactivate -= MouseUp;
            template.MouseUp -= MouseUp;
            template.LostFocus -= MouseUp;
        }
    }

    private void TemplateDisposed(object? sender, EventArgs e)
    {
        if (template is not null)
        {
            template.FormClosed -= TemplateDisposed;
            template.Disposed -= TemplateDisposed;
        }
        Detach();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            TemplateDisposed(null, EventArgs.Empty);
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        if (Frame?.Template is System.Windows.Forms.Form template)
        {
            this.template = template;
            shouldBeTransparent = true;
            MouseUp(null, EventArgs.Empty);
        }

        Attach();
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        Detach();
    }
}

/// <summary>
/// 
/// </summary>
public class XenialDevToolsWindowController : WindowController
{
    public WinWindow? DevToolsWindow { get; private set; }

    public XenialDevToolsWindowController()
    {
        TargetWindowType = WindowType.Main;
    }

    public void OpenDevTools(View view)
    {
        if (DevToolsWindow is null)
        {
            DevToolsWindow = new WinWindow(Application, TemplateContext.View, new Controller[]
            {
                Application.CreateController<FillActionContainersController>(),
                Application.CreateController<ActionControlsSiteController>(),
                Application.CreateController<XenialDevToolsController>()
            }, false, false);
        }

        var template = new DetailFormV2();

        var barManagerHolder = (IBarManagerHolder)template;

        barManagerHolder.BarManager.MainMenu.Visible = false;
        barManagerHolder.BarManager.StatusBar.Visible = false;
        barManagerHolder.BarManager.AllowCustomization = false;
        barManagerHolder.BarManager.AllowQuickCustomization = false;
        barManagerHolder.BarManager.AllowCustomization = false;

        foreach (Bar bar in barManagerHolder.BarManager.Bars)
        {
            bar.OptionsBar.DrawDragBorder = false;
        }

        DevToolsWindow.SetTemplate(template);

        DevToolsWindow.SetView(view, true, null, false);
        template.ShowMdiChildCaptionInParentTitle = false;
        template.Text = "Xenial-DevTools";

        DevToolsWindow.Show();

        var method = typeof(Controller).GetMethod("SetFrame", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        //protected internal void SetFrame(Frame frame)

        if (method is not null)
        {
            foreach (var controller in DevToolsWindow.Controllers)
            {
                method.Invoke(controller, new object[] { DevToolsWindow });
                if (controller is WindowController dialogController)
                {
                    dialogController.SetWindow(DevToolsWindow);
                }
            }
        }
    }
}
