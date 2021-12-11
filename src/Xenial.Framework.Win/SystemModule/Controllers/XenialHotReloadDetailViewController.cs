#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Framework.Layouts;
using Xenial.Framework.Model.GeneratorUpdaters;
using Xenial.Framework.SystemModule;

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

/// <summary>
/// For internal use only.
/// </summary>
public sealed class XenialHotReloadDetailViewController : ViewController
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    protected override void OnActivated()
    {
        base.OnActivated();
        XenialHotReloadManager.UpdateApp -= HotReloadManager_UpdateApp;
        XenialHotReloadManager.UpdateApp += HotReloadManager_UpdateApp;
    }

    private void HotReloadManager_UpdateApp(object? sender, XenialHotReloadEventArgs e)
    {
        var builderAttributes = View.ObjectTypeInfo.FindAttributes<DetailViewLayoutBuilderAttribute>();
        var builderAttribute = builderAttributes.FirstOrDefault(a => a.GeneratorType == e.Type);
        if (builderAttribute is null)
        {
            return;
        }
        if (View.Model is IModelDetailView detailView)
        {
            var builder = new ModelDetailViewLayoutNodesGeneratorUpdater();

            var layout = (ModelNode)detailView.Layout;
            builder.UpdateNode(layout);
            View.SaveModel();
            if (View.Control is System.Windows.Forms.Control control)
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(() => UpdateLayout(Application, Frame));
                }
                else
                {
                    UpdateLayout(Application, Frame);
                }

                static void UpdateLayout(XafApplication application, Frame frame)
                {
                    var view = frame.View;
                    var delayedViewItemsInitialization = view is CompositeView compositeView
                        ? compositeView.DelayedItemsInitialization
                        : application.DelayedViewItemsInitialization;

                    delayedViewItemsInitialization = false;

                    if (view.Control is DevExpress.ExpressApp.Win.Layout.XafLayoutControl control)
                    {
                        control.BeginUpdate();
                        try
                        {
                            view.LoadModel(delayedViewItemsInitialization);
                        }
                        finally
                        {
                            if (!delayedViewItemsInitialization)
                            {
                                view.CreateControls();
                            }
                            control.EndUpdate();
                        }
                    }
                    else
                    {
                        if (frame.SetView(null, true, null, false))
                        {
                            view.LoadModel(delayedViewItemsInitialization);
                            frame.SetView(view);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    protected override void OnDeactivated()
    {
        XenialHotReloadManager.UpdateApp -= HotReloadManager_UpdateApp;
        base.OnDeactivated();
    }
}
#endif
