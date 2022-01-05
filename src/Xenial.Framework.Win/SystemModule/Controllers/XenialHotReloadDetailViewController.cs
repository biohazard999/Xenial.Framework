using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;

using Xenial.Framework.Layouts;
using Xenial.Framework.Model.GeneratorUpdaters;
using Xenial.Framework.SystemModule;

namespace Xenial.Framework.Win.SystemModule;

/// <summary>
/// For internal use only.
/// </summary>
public sealed class XenialHotReloadDetailViewController : ViewController
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    public SimpleAction ReloadCurrentViewSimpleAction { get; }

    /// <summary>
    /// For internal use only.
    /// </summary>
    public XenialHotReloadDetailViewController()
    {
        ReloadCurrentViewSimpleAction = new SimpleAction(this, $"{GetType().FullName}.{nameof(ReloadCurrentViewSimpleAction)}", DevExpress.Persistent.Base.PredefinedCategory.Tools)
        {
            Caption = "Reload View",
            ImageName = "Action_HotReload",
            Shortcut = "CtrlShiftF3"
        };
        ReloadCurrentViewSimpleAction.Execute += ReloadCurrentViewSimpleAction_Execute;
    }

    private void ReloadCurrentViewSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        => ReloadCurrentView(null);

#if NET6_0_OR_GREATER
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
        => ReloadCurrentView(e.Type);
#endif

    /// <summary>    /// 
    /// For internal use only.
    /// </summary>
    /// <param name="type"></param>
    public void ReloadCurrentView(Type? type)
    {
        if (View is DetailView detailView)
        {
            ReloadCurrentView(Application, detailView, Frame, type);
        }
        if (View is ListView listView)
        {
            ReloadCurrentView(Application, listView, Frame, type);
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <param name="application"></param>
    /// <param name="frame"></param>
    /// <param name="view"></param>
    /// <param name="generatorType"></param>
    public static void ReloadCurrentView(XafApplication application, DetailView view, Frame frame, Type? generatorType)
    {
        if (application is null || view is null || frame is null)
        {
            return;
        }

        var builderAttributes = view.ObjectTypeInfo.FindAttributes<DetailViewLayoutBuilderAttribute>();
        var builderAttribute = generatorType is null
            ? builderAttributes.FirstOrDefault()
            : builderAttributes.FirstOrDefault(a => a.GeneratorType == generatorType);

        if (builderAttribute is null)
        {
            return;
        }

        if (view.Model is IModelDetailView detailView)
        {
            var itemsBuilder = new ModelDetailViewLayoutModelDetailViewItemsNodesGenerator();
            var builder = new ModelDetailViewLayoutNodesGeneratorUpdater();

            var items = (ModelNode)detailView.Items;
            itemsBuilder.UpdateNode(items);
            var layout = (ModelNode)detailView.Layout;
            builder.UpdateNode(layout);
            view.SaveModel();
            if (view.Control is System.Windows.Forms.Control control)
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(() => UpdateLayout(application, frame));
                }
                else
                {
                    UpdateLayout(application, frame);
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
    /// <param name="application"></param>
    /// <param name="frame"></param>
    /// <param name="view"></param>
    /// <param name="generatorType"></param>
    public static void ReloadCurrentView(XafApplication application, ListView view, Frame frame, Type? generatorType)
    {
        if (application is null || view is null || frame is null)
        {
            return;
        }

        var builderAttributes = view.ObjectTypeInfo.FindAttributes<ColumnsBuilderAttribute>();
        var builderAttribute = generatorType is null
            ? builderAttributes.FirstOrDefault()
            : builderAttributes.FirstOrDefault(a => a.GeneratorType == generatorType);

        if (builderAttribute is null)
        {
            return;
        }

        if (view.Model is IModelListView listView && application is WinApplication winApplication && winApplication.MainWindow is WinWindow winWindow)
        {
            if (winWindow.Form is System.Windows.Forms.Control control)
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(() => UpdateLayout(application, frame, listView));
                }
                else
                {
                    UpdateLayout(application, frame, listView);
                }

                static void UpdateLayout(XafApplication application, Frame frame, IModelListView listView)
                {
                    var itemsBuilder = new ModelColumnsBuilderNodesGeneratorUpdater();
                    var shortcut = frame.View.CreateShortcut();

                    if (frame.SetView(null))
                    {
                        var items = (ModelNode)listView.Columns;
                        itemsBuilder.UpdateNode(items);
                        frame.SetView(application.ProcessShortcut(shortcut), true, frame);
                    }
                }
            }
        }
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// For internal use only.
    /// </summary>
    protected override void OnDeactivated()
    {
        XenialHotReloadManager.UpdateApp -= HotReloadManager_UpdateApp;
        base.OnDeactivated();
    }
#endif
}
