using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraBars;

using Xenial.FeatureCenter.Module.BusinessObjects.Badges;
using Xenial.Framework.Badges.Win;

namespace Xenial.FeatureCenter.Module.Win
{
    public sealed class BadgedFeatureController : ViewController
    {
        public SimpleAction ToggleBadgesSimpleAction { get; }
        private bool showBadges = true;
        public BadgedFeatureController()
        {
            TargetObjectType = typeof(BadgesIntroductionDemo);
            ToggleBadgesSimpleAction = new SimpleAction(this, nameof(ToggleBadgesSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.View)
            {
                Caption = "Hide Badges",
                ImageName = "BringToFrontOfText",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
            };

            ToggleBadgesSimpleAction.Execute += ToggleBadgesSimpleAction_Execute;
            ToggleBadgesSimpleAction.CustomizeControl += ToggleBadgesSimpleAction_CustomizeControl;
        }

        private void ToggleBadgesSimpleAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is BarButtonItem barButtonItem)
            {
                barButtonItem.ButtonStyle = BarButtonStyle.Check;
            }
        }

        protected override void OnActivated()
        {
            ShowBadges(showBadges);
            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            ShowBadges(true);
            base.OnDeactivated();
        }

        private void ToggleBadgesSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            showBadges = !showBadges;
            ShowBadges(showBadges);
        }

        private void ShowBadges(bool showBadges)
        {
            ToggleBadgesSimpleAction.Caption = showBadges
                ? "Hide Badges"
                : "Show Badges";

            ToggleBadgesSimpleAction.ToolTip = showBadges
                ? "Hides badges from the navigation panel"
                : "Shows badges in the navigation panel";

            var adornerWindowsFormsCustomizeNavigationController = Frame.GetController<AdornerWindowsFormsCustomizeNavigationController>();

            if (adornerWindowsFormsCustomizeNavigationController is not null)
            {
                adornerWindowsFormsCustomizeNavigationController.Active[nameof(ToggleBadgesSimpleAction)] = showBadges;
            }

            if (Application is not null && Application.MainWindow is not null)
            {
                var mainWindowadornerWindowsFormsCustomizeNavigationController = Application.MainWindow.GetController<AdornerWindowsFormsCustomizeNavigationController>();
                if (mainWindowadornerWindowsFormsCustomizeNavigationController is not null)
                {
                    mainWindowadornerWindowsFormsCustomizeNavigationController.Active[nameof(ToggleBadgesSimpleAction)] = showBadges;
                }
            }
        }
    }
}
