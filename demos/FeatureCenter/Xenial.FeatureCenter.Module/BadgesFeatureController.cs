using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

using Xenial.FeatureCenter.Module.BusinessObjects.Badges;
using Xenial.Framework.Badges;

namespace Xenial.FeatureCenter.Module
{
    public class BadgesFeatureController : WindowController
    {
        public SimpleAction ToggleBadgesSimpleAction { get; }
        protected bool EnableBadges { get; private set; } = true;
        public BadgesFeatureController()
        {
            TargetWindowType = WindowType.Main;

            ToggleBadgesSimpleAction = new SimpleAction(this, nameof(ToggleBadgesSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.View)
            {
                Caption = "Hide Badges",
                ImageName = "BringToFrontOfText",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
            };

            ToggleBadgesSimpleAction.Execute += ToggleBadgesSimpleAction_Execute;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ShowBadges(EnableBadges);
        }

        protected override void OnDeactivated()
        {
            ShowBadges(true);
            base.OnDeactivated();
        }

        private void ToggleBadgesSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            EnableBadges = !EnableBadges;
            ShowBadges(EnableBadges);
        }

        private void ShowBadges(bool showBadges)
        {
            ToggleBadgesSimpleAction.Caption = showBadges
                ? "Hide Badges"
                : "Show Badges";

            ToggleBadgesSimpleAction.ToolTip = showBadges
                ? "Hides badges from the navigation panel"
                : "Shows badges in the navigation panel";

            var adornerWindowsFormsCustomizeNavigationController = Frame.GetController<XenialBadgesNavigationWindowControllerBase>();

            if (adornerWindowsFormsCustomizeNavigationController is not null)
            {
                adornerWindowsFormsCustomizeNavigationController.Active[nameof(ToggleBadgesSimpleAction)] = showBadges;
            }

            if (Application is not null && Application.MainWindow is not null)
            {
                var mainWindowAdornerWindowsFormsCustomizeNavigationController = Application.MainWindow.GetController<XenialBadgesNavigationWindowControllerBase>();
                if (mainWindowAdornerWindowsFormsCustomizeNavigationController is not null)
                {
                    mainWindowAdornerWindowsFormsCustomizeNavigationController.Active[nameof(ToggleBadgesSimpleAction)] = showBadges;
                }
            }
        }
    }
}
