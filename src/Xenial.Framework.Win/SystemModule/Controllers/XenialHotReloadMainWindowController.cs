using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Xenial.Framework.Win.SystemModule;

/// <summary>
/// For internal use only.
/// </summary>
public sealed class XenialHotReloadMainWindowController : WindowController
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    public XenialHotReloadMainWindowController()
        => TargetWindowType = WindowType.Main;

    /// <summary>
    /// For internal use only.
    /// </summary>
    protected override void OnActivated()
    {
        base.OnActivated();
        var controller = Frame.GetController<WindowTemplateController>();
        if (controller is not null)
        {
            controller.CustomizeWindowStatusMessages -= Controller_CustomizeWindowStatusMessages;
            controller.CustomizeWindowStatusMessages += Controller_CustomizeWindowStatusMessages;
        }
    }

    private void Controller_CustomizeWindowStatusMessages(object? sender, CustomizeWindowStatusMessagesEventArgs e)
    {
        var hotreloadPipeName = System.Environment.GetEnvironmentVariable("DOTNET_HOTRELOAD_NAMEDPIPE_NAME");
        if (!string.IsNullOrEmpty(hotreloadPipeName))
        {
            e.StatusMessages.Add("Hot-Reload enabled");
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    protected override void OnDeactivated()
    {
        var controller = Frame.GetController<WindowTemplateController>();
        if (controller is not null)
        {
            controller.CustomizeWindowStatusMessages -= Controller_CustomizeWindowStatusMessages;
        }
        base.OnDeactivated();
    }
}
