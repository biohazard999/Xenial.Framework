
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Navigation;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.Forms;
using DevExpress.XtraNavBar.ViewInfo;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xenial.Framework.Badges.Win.Helpers;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal class NavBarAdornerAdapter : AdornerAdapterBase
    {
        private readonly NavBarControl navBarControl;
        private readonly Dictionary<NavPaneForm, AdornerUIManager> navePaneFormCollection = new();
        private readonly Dictionary<NavPaneForm, List<(NavBarItemLink navBarItemLink, Badge badge)>> navePaneFormBadgeCollection = new();
        protected override Control DefaultTargetElement => navBarControl;

        public NavBarAdornerAdapter(NavBarControl navBarControl) : base(new AdornerUIManager())
        {
            this.navBarControl = navBarControl;
            navBarControl.Disposed += NavBarControl_Disposed;
        }

        private void NavBarControl_Disposed(object sender, EventArgs e)
        {
            navBarControl.Disposed -= NavBarControl_Disposed;
            Dispose();
        }

        public override void Enable(ShowNavigationItemController showNavigationItemController)
        {
            base.Enable(showNavigationItemController);
            AttachToEvents();
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
            => BeginInvokeAction(() => UpdateBadges());

        private void BeginInvokeAction(Action action)
        {
            if (navBarControl.IsHandleCreated)
            {
                navBarControl.BeginInvoke(new MethodInvoker(() =>
                {
                    action();
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
                    BeginInvokeAction(action);
                }
            }
        }

        private void UpdateBadges(bool needCalc = false)
        {
            AdornerUIManager.BeginUpdate();
            try
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
                        if (BadgeCollection.TryGetValue(choiceActionItem, out var groupBadge))
                        {
                            groupBadge.Visible = false;
                            UpdateGroupBadge(navBarViewInfo, navBarGroup, groupBadge, needCalc);

                        }
                    }
                    foreach (var (choiceActionItem, navBarItemLink) in xafNavBarControl.ActionControl.ActionItemToNavBarItemLinkMap)
                    {
                        if (BadgeCollection.TryGetValue(choiceActionItem, out var navBarItemLinkBadge))
                        {
                            navBarItemLinkBadge.Visible = false;
                            UpdateItemBadge(navBarViewInfo, navBarItemLink, navBarItemLinkBadge);
                        }
                    }

                    if (navBarControl.NavPaneForm is not null)
                    {
                        AttachToNavPaneFormEvents(navBarControl.NavPaneForm, xafNavBarControl);
                    }
                }
                QueueAnimationTask();
            }
            finally
            {
                AdornerUIManager.EndUpdate();
            }
        }

        private void UpdateGroupBadge(NavBarViewInfo navBarViewInfo, NavBarGroup navBarGroup, Badge groupBadge, bool needCalc)
        {
            var groupViewInfo = navBarViewInfo.GetGroupInfo(navBarGroup);
            if (groupViewInfo is not null && groupViewInfo.CaptionBounds != Rectangle.Empty)
            {
                var badgeViewInfo = groupBadge.GetViewInfo();

                if (needCalc || badgeViewInfo?.Cache == null)
                {
                    using (groupViewInfo.Graphics = navBarControl.CreateGraphics())
                    {
                        badgeViewInfo?.Calc(groupViewInfo.Cache, groupViewInfo.CaptionBounds);
                    }
                }
                var useText = UseText(navBarGroup);

                var rect = useText
                    ? groupViewInfo.CaptionBounds
                    : groupViewInfo.ImageBounds;

                if (badgeViewInfo is not null)
                {
                    var height = badgeViewInfo.Bounds.Height;
                    var width = badgeViewInfo.Bounds.Width;

                    groupBadge.Properties.Offset = new Point(rect.Right + width / 2, rect.Top + height / 2);
                    groupBadge.Visible = true;
                }
            }

            UpdateOverflowGroupBadge(navBarViewInfo, navBarGroup, groupBadge, needCalc);
        }

        private void UpdateOverflowGroupBadge(NavBarViewInfo navBarViewInfo, NavBarGroup navBarGroup, Badge groupBadge, bool needCalc)
        {
            if (
                navBarViewInfo is NavigationPaneViewInfo navPaneViewInfo
                && navPaneViewInfo.OverflowInfo is NavigationPaneOverflowPanelInfo overflowInfo
            )
            {
                var buttons = overflowInfo.Buttons
                    .OfType<NavigationPaneOverflowPanelObjectInfo>()
                    .Where(b => b.Group == navBarGroup)
                ;

                foreach (var button in buttons)
                {
                    var badgeViewInfo = groupBadge.GetViewInfo();

                    if (needCalc || badgeViewInfo?.Cache == null)
                    {
                        using (overflowInfo.Graphics = navBarControl.CreateGraphics())
                        {
                            badgeViewInfo?.Calc(overflowInfo.Cache, overflowInfo.Bounds);
                        }
                    }

                    if (badgeViewInfo is not null)
                    {
                        var height = badgeViewInfo.Bounds.Height;
                        var width = badgeViewInfo.Bounds.Width;

                        groupBadge.Properties.Offset = new Point(button.Bounds.Right - width / 2, button.Bounds.Top + height / 2);
                        groupBadge.Visible = true;
                    }
                }
            }
        }

        private void UpdateItemBadge(NavBarViewInfo navBarViewInfo, NavBarItemLink navBarItemLink, Badge navBarItemBadge)
        {
            var navBarItemLinkViewInfo = navBarViewInfo.GetLinkInfo(navBarItemLink);
            if (navBarItemLinkViewInfo is not null && navBarItemLink.Group is NavBarGroup navBarGroup)
            {
                var navBarGroupViewInfo = navBarViewInfo.GetGroupInfo(navBarGroup);
                if (navBarGroupViewInfo is not null)
                {
                    navBarItemBadge.Visible =
                       navBarGroup.Expanded
                       && navBarControl.OptionsNavPane.NavPaneState == NavPaneState.Expanded;

                    UpdateItemBadge(
                        navBarItemLinkViewInfo,
                        navBarItemLink,
                        navBarItemBadge,
                        navBarGroupViewInfo
                    );
                }
            }
        }

        private void UpdateItemBadge(
            NavLinkInfoArgs navBarItemLinkViewInfo,
            NavBarItemLink navBarItemLink,
            Badge navBarItemBadge,
            NavGroupInfoArgs navBarGroupViewInfo)
        {
            if (navBarItemLinkViewInfo is not null && navBarItemLink.Group is NavBarGroup)
            {
                var useText = UseText(navBarItemLink);

                var rect = useText
                    ? navBarItemLinkViewInfo.CaptionRectangle
                    : navBarItemLinkViewInfo.ImageRectangle;

                if (rect != Rectangle.Empty)
                {
                    if (
                        !navBarGroupViewInfo.ClientInfoBounds.Contains(navBarItemLinkViewInfo.Bounds)
                        && !navBarGroupViewInfo.ClientInfoBounds.IntersectsWith(navBarItemLinkViewInfo.Bounds)
                    )
                    {
                        navBarItemBadge.Visible = false;
                        return;
                    }
                    var badgeViewInfo = navBarItemBadge.GetViewInfo();
                    if (badgeViewInfo is not null)
                    {
                        var height = badgeViewInfo.Bounds.Height;
                        navBarItemBadge.Properties.Offset = new Point(rect.Right, rect.Top + height / 2);
                    }
                }
            }
        }

        private void AttachToNavPaneFormEvents(NavPaneForm navPaneForm, XafNavBarControl xafNavBarControl)
        {
            Dispose(() => navPaneForm.Disposed -= navPaneFormDisposed);
            navPaneForm.Disposed -= navPaneFormDisposed;
            navPaneForm.Disposed += navPaneFormDisposed;

            Dispose(() => navPaneForm.Resize -= NeedInvoke);
            navPaneForm.Resize -= NeedInvoke;
            navPaneForm.Resize += NeedInvoke;

            Dispose(() => navPaneForm.Shown -= NeedInvoke);
            navPaneForm.Shown -= NeedInvoke;
            navPaneForm.Shown += NeedInvoke;

            void navPaneFormDisposed(object sender, EventArgs e)
            {
                navPaneForm.Resize -= NeedInvoke;
                navPaneForm.Disposed -= navPaneFormDisposed;
                if (navePaneFormCollection.TryGetValue(navPaneForm, out var adornerUIManager))
                {
                    adornerUIManager.Dispose();
                    navePaneFormCollection.Remove(navPaneForm);
                }
                if (navePaneFormBadgeCollection.TryGetValue(navPaneForm, out var collection))
                {
                    collection.Clear();
                }
                navePaneFormBadgeCollection.Remove(navPaneForm);
            }

            void NeedInvoke(object sender, EventArgs e)
            {
                BeginInvokeAction(() =>
                {
                    AdornerUIManager? adornerUiManager;
                    navePaneFormCollection.TryGetValue(navPaneForm, out adornerUiManager);
                    adornerUiManager?.BeginUpdate();
                    try
                    {
                        if (navePaneFormBadgeCollection.TryGetValue(navPaneForm, out var collection))
                        {
                            foreach (var (navBarItemLink, badgeClone) in collection)
                            {
                                var navBarItemLinkInfo = navPaneForm.ViewInfo.GetLinkInfo(navBarItemLink);

                                UpdateItemBadge(
                                    navBarItemLinkInfo,
                                    navBarItemLink,
                                    badgeClone,
                                    navPaneForm.ExpandedGroupInfo
                                );
                            }
                        }
                    }
                    finally
                    {
                        adornerUiManager?.EndUpdate();
                    }
                });
            }

            AdornerUIManager adornerUIManager;
            if (!navePaneFormCollection.TryGetValue(navPaneForm, out adornerUIManager) && adornerUIManager is null)
            {
                adornerUIManager = new AdornerUIManager();
                DisposableList.Add(adornerUIManager);
                adornerUIManager.Owner = navPaneForm;
                navePaneFormCollection[navPaneForm] = adornerUIManager;

                if (navPaneForm.ExpandedGroup is not null)
                {
                    foreach (var (choiceActionItem, navBarItemLink) in
                        xafNavBarControl.ActionControl.ActionItemToNavBarItemLinkMap
                        .Where(k => navPaneForm.ExpandedGroup.ItemLinks.Contains(k.Value))
                    )
                    {
                        if (BadgeCollection.TryGetValue(choiceActionItem, out var navBarItemLinkBadge))
                        {
                            var badgeClone = (Badge)navBarItemLinkBadge.Clone();
                            badgeClone.TargetElement = navPaneForm;
                            adornerUIManager.Elements.Add(badgeClone);

                            var navBarItemLinkInfo = navPaneForm.ViewInfo.GetLinkInfo(navBarItemLink);

                            if (!navePaneFormBadgeCollection.TryGetValue(navPaneForm, out var collection) && collection is null)
                            {
                                navePaneFormBadgeCollection[navPaneForm] = new();
                            }

                            var col = navePaneFormBadgeCollection[navPaneForm];

                            col.Add((navBarItemLink, badgeClone));

                            UpdateItemBadge(
                                navBarItemLinkInfo,
                                navBarItemLink,
                                badgeClone,
                                navPaneForm.ExpandedGroupInfo
                            );
                            badgeClone.Visible = true;
                        }
                    }
                }
            }

            NeedInvoke(new(), EventArgs.Empty);
        }

        private static bool UseText(NavBarItemLink navBarItemLink)
            => navBarItemLink.Group?.GroupStyle switch
            {
                NavBarGroupStyle.Default => true,
                NavBarGroupStyle.SmallIconsText => true,
                NavBarGroupStyle.SmallIconsList => true,
                _ => false
            };

        private static bool UseText(NavBarGroup navbarGroup)
           => navbarGroup.NavBar?.OptionsNavPane.NavPaneState switch
           {
               NavPaneState.Expanded => true,
               _ => false
           };

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
    }
}
