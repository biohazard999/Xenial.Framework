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
            Caption = "Reload View"
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

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <param name="type"></param>
    public void ReloadCurrentView(Type? type)
    {
        var builderAttributes = View.ObjectTypeInfo.FindAttributes<DetailViewLayoutBuilderAttribute>();
        var builderAttribute = type is null
            ? builderAttributes.FirstOrDefault()
            : builderAttributes.FirstOrDefault(a => a.GeneratorType == type);

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
