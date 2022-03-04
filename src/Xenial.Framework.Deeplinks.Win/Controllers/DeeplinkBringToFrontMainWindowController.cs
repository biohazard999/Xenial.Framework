
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

using System;

using System.Linq;
using System.Runtime.InteropServices;

namespace Xenial.Framework.Deeplinks.Win;

/// <summary>
/// 
/// </summary>
public sealed class DeeplinkBringToFrontMainWindowController : WindowController
{
    /// <summary>
    /// 
    /// </summary>
    public DeeplinkBringToFrontMainWindowController()
        => TargetWindowType = WindowType.Main;

    /// <summary>
    /// 
    /// </summary>
    protected override void OnActivated()
    {
        base.OnActivated();
        var controller = Frame.GetController<DeeplinkMainWindowController>();
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
        var controller = Frame.GetController<DeeplinkMainWindowController>();
        if (controller is not null)
        {
            controller.ArgumentsHandled -= Controller_ArgumentsHandled;
        }
    }

    private void Controller_ArgumentsHandled(object? sender, EventArgs e)
    {
        if (Window is WinWindow winWindow && winWindow.Form is System.Windows.Forms.Form form)
        {
            if (form.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                RestoreFromMinimzied(form);
            }

            form.Activate();
            _ = SetForegroundWindow(form.Handle.ToInt32());
            form.BringToFront();
        }
    }

    #region https://stackoverflow.com/questions/5282588/how-can-i-bring-my-application-window-to-the-front

    [DllImport("User32.dll")]
    private static extern Int32 SetForegroundWindow(int hWnd);

    #endregion

    #region https://stackoverflow.com/questions/4410717/c-sharp-programmatically-unminimize-form

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "It's interopt")]
    private struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }

    private static void RestoreFromMinimzied(System.Windows.Forms.Form form)
    {
        const int WPF_RESTORETOMAXIMIZED = 0x2;
        var placement = new WINDOWPLACEMENT();
        placement.length = Marshal.SizeOf(placement);
        GetWindowPlacement(form.Handle, ref placement);

        if ((placement.flags & WPF_RESTORETOMAXIMIZED) == WPF_RESTORETOMAXIMIZED)
        {
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
        else
        {
            form.WindowState = System.Windows.Forms.FormWindowState.Normal;
        }
    }

    #endregion
}
