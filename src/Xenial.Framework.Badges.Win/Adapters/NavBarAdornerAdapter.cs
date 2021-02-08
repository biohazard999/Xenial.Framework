using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Navigation;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;

using Xenial.Framework.Badges.Win.Helpers;

using static Xenial.Framework.Badges.Win.Adapters.ActionItemBadgeFactory;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal class NavBarAdornerAdapter : IDisposable
    {
        private bool disposedValue;
        private readonly NavBarControl navBarControl;
        private readonly AdornerUIManager adornerUIManager;
        private readonly Dictionary<ChoiceActionItem, Badge> badgeCollection = new();
        private readonly DisposableList disposableList = new();

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
            AttachToEvents();
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

        private void AttachToEvents()
        {
            Dispose(() => navBarControl.MouseMove -= MouseMove);
            navBarControl.MouseMove -= MouseMove;
            navBarControl.MouseMove += MouseMove;

            void MouseMove(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    UpdateBadges(true);
                }
            }

            Dispose(() => navBarControl.GroupCollapsed -= GroupChange);
            navBarControl.GroupCollapsed -= GroupChange;
            navBarControl.GroupCollapsed += GroupChange;

            Dispose(() => navBarControl.GroupExpanded -= GroupChange);
            navBarControl.GroupExpanded -= GroupChange;
            navBarControl.GroupExpanded += GroupChange;

            void GroupChange(object sender, EventArgs e)
            {
                UpdateBadges(true);
            }

            Dispose(() => navBarControl.NavPaneStateChanged -= NeedInvoke);
            navBarControl.NavPaneStateChanged -= NeedInvoke;
            navBarControl.NavPaneStateChanged += NeedInvoke;

            Dispose(() => navBarControl.Resize -= NeedInvoke);
            navBarControl.Resize -= NeedInvoke;
            navBarControl.Resize += NeedInvoke;

            void NeedInvoke(object sender, EventArgs e)
                => BeginInvokeUpdateBadges();
        }

        private void BeginInvokeUpdateBadges()
        {
            if (navBarControl.IsHandleCreated)
            {
                navBarControl.BeginInvoke(new MethodInvoker(() =>
                {
                    UpdateBadges();
                }));
            }
            else
            {
                Dispose(() => navBarControl.HandleCreated -= HandleCreated);
                navBarControl.HandleCreated -= HandleCreated;
                navBarControl.HandleCreated += HandleCreated;

                void HandleCreated(object s, EventArgs e)
                {
                    navBarControl.HandleCreated -= HandleCreated;
                    BeginInvokeUpdateBadges();
                }
            }
        }

        private void UpdateBadges(bool needCalc = false)
        {
            var navBarViewInfo = navBarControl.GetViewInfo();

            if (navBarViewInfo is null)
            {
                return;
            }
            if (needCalc)
            {
                navBarViewInfo.Calc(navBarControl.ClientRectangle);
            }

            if (
                navBarControl is XafNavBarControl xafNavBarControl
                && xafNavBarControl.ActionControl is not null
            )
            {
                foreach (var (choiceActionItem, navBarGroup) in xafNavBarControl.ActionControl.ActionItemToNavBarGroupMap)
                {
                    if (badgeCollection.TryGetValue(choiceActionItem, out var groupBadge))
                    {
                        UpdateGroupBadge(navBarViewInfo, navBarGroup, groupBadge, needCalc);
                    }
                }
            }
            QueueAnimationTask();
        }

        private void UpdateGroupBadge(NavBarViewInfo navBarViewInfo, NavBarGroup navBarGroup, Badge groupBadge, bool needCalc)
        {
            var groupViewInfo = navBarViewInfo.GetGroupInfo(navBarGroup);
            if (groupViewInfo != null && groupViewInfo.CaptionBounds != Rectangle.Empty)
            {
                //TODO: speed up lookup
                var propertyInfoBadge = typeof(Badge).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                var badgeViewInfo = propertyInfoBadge.GetValue(groupBadge, null) as BadgeViewInfo;

                if (needCalc || badgeViewInfo?.Cache == null)
                {
                    using (groupViewInfo.Graphics = navBarControl.CreateGraphics())
                    {
                        badgeViewInfo?.Calc(groupViewInfo.Cache, groupViewInfo.CaptionBounds);
                    }
                }
                if (badgeViewInfo is not null)
                {
                    var height = badgeViewInfo.Bounds.Height;

                    groupBadge.Properties.Offset = new Point(groupViewInfo.CaptionBounds.Right, groupViewInfo.CaptionBounds.Top + height / 2);
                    groupBadge.Visible = true;
                }
            }
        }

        private void QueueAnimationTask()
        {
            if (navBarControl.OptionsNavPane.IsAnimationInProgress)
            {
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(navBarControl.OptionsNavPane.AnimationFramesCount);

                        if (!navBarControl.OptionsNavPane.IsAnimationInProgress)
                        {
                            await Task.Delay(navBarControl.OptionsNavPane.AnimationFramesCount);
                            break;
                        }
                    }
                    if (!navBarControl.OptionsNavPane.IsAnimationInProgress)
                    {
                        BeginInvokeUpdateBadges();
                    }
                }, TaskCreationOptions.AttachedToParent);
            }
        }

        public void Disable()
        {
            disposableList.Dispose();

            foreach (var badge in badgeCollection.Values)
            {
                badge.TargetElement = null;
            }

            badgeCollection.Clear();
        }

        private void Dispose(Action disposeAction) => disposableList.Actions.Add(disposeAction);
    }
}
