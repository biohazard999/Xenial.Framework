using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using Xenial.Framework.Win.SystemModule;

namespace MailClient.Module.Win;

public class CustomHRController : ViewController
{
    public CustomHRController()
        => TargetViewType = ViewType.ListView;

    protected override void OnActivated()
    {
        base.OnActivated();
        if (View is ListView listView)
        {
            var hrc = Frame.GetController<XenialHotReloadDetailViewController>();
            if (hrc is not null)
            {
                hrc.ReloadCurrentViewSimpleAction.Execute -= ReloadCurrentViewSimpleAction_Execute;
                hrc.ReloadCurrentViewSimpleAction.Execute += ReloadCurrentViewSimpleAction_Execute;
            }
        }
    }

    protected override void OnDeactivated()
    {
        var hrc = Frame.GetController<XenialHotReloadDetailViewController>();
        if (hrc is not null)
        {
            hrc.ReloadCurrentViewSimpleAction.Execute -= ReloadCurrentViewSimpleAction_Execute;
        }
        base.OnDeactivated();
    }

    private void ReloadCurrentViewSimpleAction_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
    {
        if (View.IsControlCreated && View is ListView listView && listView.EditView is DetailView detailView && listView.EditFrame is Frame nestedFrame)
        {
            XenialHotReloadDetailViewController.ReloadCurrentView(Application, detailView, nestedFrame, null);
        }
    }

    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
        if (View is ListView listView)
        {

        }
    }

}
