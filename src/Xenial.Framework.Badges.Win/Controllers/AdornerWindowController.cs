using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraNavBar;

using Xenial.Framework.Badges.Win.Adapters;
using Xenial.Framework.Badges.Win.Helpers;

namespace Xenial.Framework.Badges.Win
{
    public sealed class AdornerWindowsFormsCustomizeNavigationController : WindowController
    {
        private readonly DisposableList disposables = new();
        private readonly List<IAdornerAdapter> adornerAdapters = new();

        public AdornerWindowsFormsCustomizeNavigationController()
            => TargetWindowType = WindowType.Main;

        protected override void OnActivated()
        {
            base.OnActivated();

            var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            if (showNavigationItemController is not null)
            {
                showNavigationItemController.ShowNavigationItemAction.CustomizeControl -= ShowNavigationItemAction_CustomizeControl;
                showNavigationItemController.ShowNavigationItemAction.CustomizeControl += ShowNavigationItemAction_CustomizeControl;
                foreach (var adornerAdapter in adornerAdapters)
                {
                    adornerAdapter.Enable(showNavigationItemController);
                }
            }
        }

        private void ShowNavigationItemAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            if (showNavigationItemController is not null)
            {
                static IAdornerAdapter? FindAdornerAdapter(object control)
                    => control switch
                    {
                        AccordionControl accordionControl => new AccordionAdornerAdapter(accordionControl),
                        NavBarControl navBarControl => new NavBarAdornerAdapter(navBarControl),
                        _ => null
                    };

                var adapter = FindAdornerAdapter(e.Control);
                if (adapter is not null)
                {
                    adornerAdapters.Add(adapter);
                    disposables.Add(adapter);
                    adapter.Enable(showNavigationItemController);
                }
            }
        }

        protected override void OnDeactivated()
        {
            foreach (var adornerAdapter in adornerAdapters)
            {
                adornerAdapter.Disable();
            }

            var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            if (showNavigationItemController is not null)
            {
                showNavigationItemController.ShowNavigationItemAction.CustomizeControl -= ShowNavigationItemAction_CustomizeControl;
            }

            base.OnDeactivated();
        }

        public void Destroy()
            => disposables.Dispose();

        protected override void Dispose(bool disposing)
        {
            Destroy();
            base.Dispose(disposing);
        }
    }
}
