using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraNavBar;

using static Xenial.Framework.Badges.Win.Adapters.ActionItemBadgeFactory;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal class NavBarAdornerAdapter : IDisposable
    {
        private bool disposedValue;
        private readonly NavBarControl navBarControl;
        private readonly AdornerUIManager adornerUIManager;
        private readonly Dictionary<ChoiceActionItem, Badge> badgeCollection = new();

        public NavBarAdornerAdapter(NavBarControl navBarControl)
        {
            this.navBarControl = navBarControl;
            adornerUIManager = new AdornerUIManager();
            adornerUIManager.Owner = navBarControl.FindForm();
            navBarControl.Disposed += NavBarControl_Disposed;
        }

        private void NavBarControl_Disposed(object sender, EventArgs e)
        {
            navBarControl.Disposed -= NavBarControl_Disposed;
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    adornerUIManager.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Enable(ShowNavigationItemController showNavigationItemController)
        {
            CollectBadges(showNavigationItemController.ShowNavigationItemAction.Items);
            _ = true;
        }

        private void CollectBadges(ChoiceActionItemCollection choiceActionItems)
        {
            adornerUIManager.BeginUpdate();
            try
            {
                foreach (var choiceActionItem in choiceActionItems)
                {
                    CollectBadge(choiceActionItem);
                    CollectBadges(choiceActionItem.Items);
                }

                void CollectBadge(ChoiceActionItem choiceActionItem)
                {
                    var badge = CreateBadge(choiceActionItem);
                    if (badge is not null)
                    {
                        badge.Visible = false;
                        badge.TargetElement = navBarControl;
                        adornerUIManager.Elements.Add(badge);
                        badgeCollection[choiceActionItem] = badge;
                    }
                }
            }
            finally
            {
                adornerUIManager.EndUpdate();
            }
        }

        public void Disable()
        {
            badgeCollection.Clear();
            _ = true;
        }
    }
}
