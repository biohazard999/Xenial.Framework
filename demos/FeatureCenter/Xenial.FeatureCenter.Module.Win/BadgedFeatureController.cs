using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

using Xenial.FeatureCenter.Module.BusinessObjects.Badges;

namespace Xenial.FeatureCenter.Module.Win
{
    public sealed class BadgedFeatureController : ViewController
    {
        public SimpleAction ToggleBadgesSimpleAction { get; }
        private bool toggle;
        public BadgedFeatureController()
        {
            TargetObjectType = typeof(BadgesIntroductionDemo);
            ToggleBadgesSimpleAction = new SimpleAction(this, nameof(ToggleBadgesSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.View)
            {
                Caption = "Toggle Badges",
                ImageName = "BringToFrontOfText",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
            };

            ToggleBadgesSimpleAction.Execute += ToggleBadgesSimpleAction_Execute;
        }

        private void ToggleBadgesSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var adornerWindowsFormsCustomizeNavigationController = Frame.GetController<Xenial.Framework.Badges.Win.AdornerWindowsFormsCustomizeNavigationController>();

            if (adornerWindowsFormsCustomizeNavigationController is not null)
            {
                adornerWindowsFormsCustomizeNavigationController.Active[nameof(ToggleBadgesSimpleAction)] = toggle;
            }

            if (Application is not null && Application.MainWindow is not null)
            {
                var mainWindowadornerWindowsFormsCustomizeNavigationController = Application.MainWindow.GetController<Xenial.Framework.Badges.Win.AdornerWindowsFormsCustomizeNavigationController>();
                if (mainWindowadornerWindowsFormsCustomizeNavigationController is not null)
                {
                    mainWindowadornerWindowsFormsCustomizeNavigationController.Active[nameof(ToggleBadgesSimpleAction)] = toggle;
                }
            }

            toggle = !toggle;

        }
    }
}
