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

            var windowTemplateController = FindWindowTemplateController();
            if (windowTemplateController is not null)
            {
                windowTemplateController.CustomizeWindowStatusMessages -= WindowTemplateController_CustomizeWindowStatusMessages;
                windowTemplateController.CustomizeWindowStatusMessages += WindowTemplateController_CustomizeWindowStatusMessages;
            }
        }

        private WindowTemplateController? FindWindowTemplateController()
        {
            if (Application.ShowViewStrategy is MdiShowViewStrategy && Application.MainWindow is Window mainWindow)
            {
                return mainWindow.GetController<WindowTemplateController>();
            }

            if (Frame is Frame frame)
            {
                return frame.GetController<WindowTemplateController>();
            }
            return null;
        }

        protected override void OnDeactivated()
        {
            var windowTemplateController = FindWindowTemplateController();
            if (windowTemplateController is not null)
            {
                windowTemplateController.CustomizeWindowStatusMessages -= WindowTemplateController_CustomizeWindowStatusMessages;
            }

            base.OnDeactivated();
        }

        private void WindowTemplateController_CustomizeWindowStatusMessages(object? sender, CustomizeWindowStatusMessagesEventArgs e)
        {
            foreach (var versionInfo in FeatureCenterModule.VersionInformation)
            {
                e.StatusMessages.Add(versionInfo);
            }
        }
    }
}
