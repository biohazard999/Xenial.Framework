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
    /// <summary>
    /// A controller for handling badges windows forms navigation windows. This class cannot be
    /// inherited./.
    /// </summary>
    ///
    /// <seealso cref="XenialBadgesNavigationWindowControllerBase"/>

    [XenialCheckLicence]
    public sealed partial class XenialBadgesWindowsFormsNavigationWindowController : XenialBadgesNavigationWindowControllerBase
    {
        private readonly DisposableList disposables = new();
        private readonly List<IAdornerAdapter> adornerAdapters = new();

        /// <summary>   Executes the 'activated' action. </summary>
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


        private void ShowNavigationItemAction_CustomizeControl(object? sender, CustomizeControlEventArgs e)
        {
            var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            if (showNavigationItemController is not null)
            {
                static IAdornerAdapter? FindAdornerAdapter(object? control)
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

        /// <summary>   Called when [deactivated]. </summary>
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

        /// <summary>   Destroys this instance. </summary>
        public void Destroy()
            => disposables.Dispose();

        /// <summary>   Disposes the specified disposing. </summary>
        ///
        /// <param name="disposing">    The disposing. </param>

        protected override void Dispose(bool disposing)
        {
            Destroy();
            base.Dispose(disposing);
        }
    }
}
