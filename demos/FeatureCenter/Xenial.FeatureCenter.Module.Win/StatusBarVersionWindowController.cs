using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;

namespace Xenial.FeatureCenter.Module.Win
{
    public class StatusBarVersionWindowController : WindowController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            WindowTemplateController? windowTemplateController;
            if (Application.ShowViewStrategy is MdiShowViewStrategy)
            {
                windowTemplateController = Application.MainWindow.GetController<WindowTemplateController>();
            }
            else
            {
                windowTemplateController = Frame.GetController<WindowTemplateController>();
            }
            if (windowTemplateController is not null)
            {
                windowTemplateController.CustomizeWindowStatusMessages -= WindowTemplateController_CustomizeWindowStatusMessages;
                windowTemplateController.CustomizeWindowStatusMessages += WindowTemplateController_CustomizeWindowStatusMessages;
            }
        }

        protected override void OnDeactivated()
        {
            WindowTemplateController? windowTemplateController;
            if (Application.ShowViewStrategy is MdiShowViewStrategy)
            {
                windowTemplateController = Application.MainWindow.GetController<WindowTemplateController>();
            }
            else
            {
                windowTemplateController = Frame.GetController<WindowTemplateController>();
            }
            if (windowTemplateController is not null)
            {
                windowTemplateController.CustomizeWindowStatusMessages -= WindowTemplateController_CustomizeWindowStatusMessages;
            }
            base.OnDeactivated();
        }

        private void WindowTemplateController_CustomizeWindowStatusMessages(object sender, CustomizeWindowStatusMessagesEventArgs e)
        {
            foreach (var versionInfo in FeatureCenterModule.VersionInformation)
            {
                e.StatusMessages.Add(versionInfo);
            }
        }
    }
}
