using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils.VisualEffects;

using Xenial.Framework.Badges.Win.Helpers;

using static Xenial.Framework.Badges.Win.Adapters.ActionItemBadgeFactory;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal abstract class AdornerAdapterBase : IAdornerAdapter
    {
        protected readonly Dictionary<ChoiceActionItem, Badge> BadgeCollection = new();
        protected readonly DisposableList DisposableList = new();
        protected readonly AdornerUIManager AdornerUIManager;
        private bool disposedValue;

        protected abstract Control DefaultTargetElement { get; }

        public AdornerAdapterBase(AdornerUIManager adornerUIManager)
            => AdornerUIManager = adornerUIManager;

        public void Disable()
        {
            DisposableList.Dispose();

            foreach (var badge in BadgeCollection.Values)
            {
                badge.TargetElement = null;
            }

            BadgeCollection.Clear();
        }

        public virtual void Enable(ShowNavigationItemController showNavigationItemController)
        {
            AdornerUIManager.Owner = DefaultTargetElement.FindForm();
            DefaultTargetElement.Disposed -= DefaultTargetElement_Disposed;
            DefaultTargetElement.Disposed += DefaultTargetElement_Disposed;
            CollectBadges(showNavigationItemController.ShowNavigationItemAction.Items);
        }

        private void CollectBadges(ChoiceActionItemCollection choiceActionItems)
        {
            AdornerUIManager.BeginUpdate();
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
                        badge.TargetElement = DefaultTargetElement;
                        AdornerUIManager.Elements.Add(badge);
                        BadgeCollection[choiceActionItem] = badge;
                    }
                }
            }
            finally
            {
                AdornerUIManager.EndUpdate();
            }
        }

        private void DefaultTargetElement_Disposed(object? sender, EventArgs e)
        {
            Disable();
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disable();
                    AdornerUIManager.Owner = null;
                    AdornerUIManager.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(Action disposeAction) => DisposableList.Actions.Add(disposeAction);

        protected void BeginInvokeAction(Action action)
        {
            if (DefaultTargetElement.IsHandleCreated)
            {
                DefaultTargetElement.BeginInvoke(new MethodInvoker(() =>
                {
                    action();
                }));
            }
            else
            {
                Dispose(() => DefaultTargetElement.HandleCreated -= HandleCreated);
                DefaultTargetElement.HandleCreated -= HandleCreated;
                DefaultTargetElement.HandleCreated += HandleCreated;

                void HandleCreated(object? s, EventArgs e)
                {
                    DefaultTargetElement.HandleCreated -= HandleCreated;
                    BeginInvokeAction(action);
                }
            }
        }
    }
}
