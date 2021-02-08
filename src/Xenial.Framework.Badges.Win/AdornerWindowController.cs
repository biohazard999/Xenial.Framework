using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Navigation;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.XtraTreeList;

using Xenial.Framework.Badges.Model;
using Xenial.Framework.Badges.Win.Adapters;
using Xenial.Framework.Badges.Win.Helpers;

using static Xenial.Framework.Badges.Win.Helpers.ModelMapperExtensions;

namespace Xenial.FeatureCenter.Module.Win
{
    public class AdornerWindowsFormsCustomizeNavigationController : WindowController
    {
        private readonly DisposableList disposables = new();

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
            }
        }

        private TreeList? FindEmbeddedTreeList(NavBarGroupControlContainer container)
        {
            if (container is not null && container.Controls.Count >= 1)
            {
                return container.Controls.OfType<TreeList>().FirstOrDefault();
            }
            return null;
        }

        private void CustomizeEmbeddedTreeList(AdornerUIManager adorner, NavGroupCollection groups)
        {
            foreach (NavBarGroup group in groups)
            {
                if (
                    group.NavBar is XafNavBarControl xafNavBarControl
                    && xafNavBarControl.ActionControl is not null
                )
                {
                    if (xafNavBarControl.ActionControl.NavBarGroupToActionItemMap.TryGetValue(group, out var actionItem))
                    {
                        if (
                            actionItem.Model is IXenialModelBadgeStaticTextItem modelBadgeStaticTextItem
                            && modelBadgeStaticTextItem.XenialBadgeStaticText is not null
                        )
                        {
                            var badge = new Badge()
                            {
                                Visible = true,
                                Properties =
                                {
                                    Text = modelBadgeStaticTextItem.XenialBadgeStaticText,
                                    PaintStyle = modelBadgeStaticTextItem.XenialBadgeStaticPaintStyle.ConvertPaintStyle()
                                }
                            };

                            badge.TargetElement = group.NavBar;
                            badge.Tag = group;
                            group.Tag = badge;
                            adorner.Elements.Add(badge);
                        }
                    }
                    foreach (var navBarItemLink in group.VisibleItemLinks.OfType<NavBarItemLink>())
                    {
                        if (xafNavBarControl.ActionControl.NavBarItemLinkToActionItemMap.TryGetValue(navBarItemLink, out var actionItemLink))
                        {
                            if (
                                actionItemLink.Model is IXenialModelBadgeStaticTextItem modelBadgeStaticTextItem
                                && modelBadgeStaticTextItem.XenialBadgeStaticText is not null
                            )
                            {
                                var badge = new Badge()
                                {
                                    Visible = true,
                                    Properties =
                                    {
                                        Text = modelBadgeStaticTextItem.XenialBadgeStaticText,
                                        PaintStyle =  modelBadgeStaticTextItem.XenialBadgeStaticPaintStyle.ConvertPaintStyle()
                                    }
                                };

                                badge.TargetElement = group.NavBar;
                                badge.Tag = navBarItemLink;
                                navBarItemLink.Item.Tag = badge;
                                adorner.Elements.Add(badge);
                            }
                        }
                    }
                }
                var treeList = FindEmbeddedTreeList(group.ControlContainer);
            }
        }

        private void ShowNavigationItemAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is AccordionControl accordionControl)
            {
                var form = accordionControl.FindForm();

                if (form is not null)
                {
                    var adorner = new AdornerUIManager();
                    adorner.Owner = form;
                }
                if (accordionControl is XafAccordionControl xafAccordionControl)
                {
                    foreach (var element in accordionControl.Elements)
                    {
                        var attachedAction2 = element.Tag as ChoiceActionItem;
                        foreach (var el in element.Elements)
                        {
                            var attachedAction = el.Tag as ChoiceActionItem;
                        }
                    }
                }
            }
            else if (e.Control is NavBarControl navBarControl)
            {
                var adapter = new NavBarAdornerAdapter(navBarControl);
                var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
                if (showNavigationItemController is not null)
                {
                    adapter.Enable(showNavigationItemController);
                    return;
                };

                var form = navBarControl.FindForm();

                if (form is not null)
                {
                    var adorner = new AdornerUIManager();
                    adorner.Owner = form;
                    CustomizeEmbeddedTreeList(adorner, navBarControl.Groups);

                    void BeginInvokeUpdateBadges()
                    {
                        if (navBarControl.IsHandleCreated)
                        {
                            navBarControl.BeginInvoke(new MethodInvoker(() =>
                            {
                                UpdateBadges(sender);
                            }));
                        }
                        else
                        {
                            navBarControl.HandleCreated -= HandleCreated;
                            navBarControl.HandleCreated += HandleCreated;

                            void HandleCreated(object s, EventArgs e)
                            {
                                navBarControl.HandleCreated -= HandleCreated;
                                BeginInvokeUpdateBadges();
                            }
                        }
                    }

                    navBarControl.MouseMove += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            UpdateBadges(navBarControl, true);
                        }
                    };

                    navBarControl.GroupCollapsed += (s, e) =>
                    {
                        UpdateBadges(navBarControl, true);
                    };

                    navBarControl.GroupExpanded += (s, e) =>
                    {
                        UpdateBadges(navBarControl, true);
                    };

                    navBarControl.MouseWheel += (s, e) =>
                    {
                        BeginInvokeUpdateBadges();
                    };

                    navBarControl.NavPaneStateChanged += (s, e) =>
                    {
                        BeginInvokeUpdateBadges();
                    };

                    navBarControl.Resize += (s, e) =>
                    {
                        BeginInvokeUpdateBadges();
                    };

                    void UpdateBadges(object sender, bool needCalc = false)
                    {
                        var x = navBarControl.GetViewInfo(); //TODO: GetViewInfo
                        _ = x;
                        var pi = typeof(NavBarControl).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                        var viewInfo = pi.GetValue(navBarControl, null);
                        if (viewInfo == null)
                        {
                            return;
                        }
                        if (needCalc)
                        {
                            if (viewInfo is NavBarViewInfo vi)
                            {
                                vi.Calc(navBarControl.ClientRectangle);
                            }
                        }

                        foreach (NavBarGroup g in navBarControl.Groups)
                        {
                            if (g.Tag is Badge b)
                            {
                                if (viewInfo is NavBarViewInfo vi)
                                {
                                    var gi = vi.GetGroupInfo(g);
                                    if (gi != null && gi.CaptionBounds != Rectangle.Empty)
                                    {
                                        var piBadge = typeof(Badge).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                                        var viBadge = piBadge.GetValue(b, null) as BadgeViewInfo;
                                        if (needCalc || viBadge?.Cache == null)
                                        {
                                            using (gi.Graphics = navBarControl.CreateGraphics())
                                            {
                                                viBadge?.Calc(gi.Cache, gi.CaptionBounds);
                                            }
                                        }
                                        if (viBadge is not null)
                                        {
                                            var height = viBadge.Bounds.Height;

                                            b.Properties.Offset = new Point(gi.CaptionBounds.Right, gi.CaptionBounds.Top + height / 2);
                                        }
                                    }
                                }


                                if (viewInfo is DevExpress.XtraNavBar.ViewInfo.NavigationPaneViewInfo viPanel)
                                {
                                    if (viPanel.OverflowInfo != null)
                                    {
                                        foreach (NavigationPaneOverflowPanelObjectInfo button in viPanel.OverflowInfo.Buttons)
                                        {
                                            if (button.HintText != g.Caption)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                var piBadge = typeof(Badge).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                                                var viBadge = piBadge.GetValue(b, null) as BadgeViewInfo;
                                                if (needCalc || viBadge?.Cache == null)
                                                {
                                                    using (viPanel.OverflowInfo.Graphics = navBarControl.CreateGraphics())
                                                    {
                                                        viBadge?.Calc(viPanel.OverflowInfo.Cache, viPanel.OverflowInfo.Bounds);
                                                    }
                                                }
                                                if (viBadge is not null)
                                                {
                                                    var height = viBadge.Bounds.Height;
                                                    var width = viBadge.Bounds.Width;

                                                    b.Properties.Offset = new Point(button.Bounds.Right - width / 2, button.Bounds.Top + height / 2);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (viewInfo is NavBarViewInfo vi2)
                            {
                                foreach (var visibleItemLink in g.VisibleItemLinks.OfType<NavBarItemLink>())
                                {
                                    var li = vi2.GetLinkInfo(visibleItemLink);
                                    var tag = visibleItemLink.Item.Tag;
                                    var liB = tag as Badge;
                                    if (liB is not null)
                                    {
                                        liB.Visible = g.Expanded && navBarControl.OptionsNavPane.NavPaneState == NavPaneState.Expanded;

                                        var useText = g.GroupStyle == NavBarGroupStyle.Default
                                            || g.GroupStyle == NavBarGroupStyle.SmallIconsText
                                            || g.GroupStyle == NavBarGroupStyle.SmallIconsList;

                                        if (li != null)
                                        {
                                            var rect = useText
                                                ? li.CaptionRectangle
                                                : li.ImageRectangle;

                                            if (g.NavBar.NavPaneForm is not null)
                                            {
                                                if (g.NavBar.NavPaneForm.Tag is not AdornerUIManager)
                                                {
                                                    var a = new AdornerUIManager();
                                                    g.NavBar.NavPaneForm.Tag = a;
                                                    a.Owner = g.NavBar.NavPaneForm;
                                                    g.NavBar.NavPaneForm.Disposed -= NavPaneFormDisposed;
                                                    g.NavBar.NavPaneForm.Disposed += NavPaneFormDisposed;
                                                    void NavPaneFormDisposed(object sender, EventArgs e)
                                                    {
                                                        a.Dispose();
                                                        g.NavBar.NavPaneForm.Disposed -= NavPaneFormDisposed;
                                                    }
                                                }
                                                if (g.NavBar.NavPaneForm.Tag is AdornerUIManager navPaneFormAdorner)
                                                {
                                                    var li2 = g.NavBar.NavPaneForm.ViewInfo.GetLinkInfo(visibleItemLink);
                                                    if (li2 is not null)
                                                    {
                                                        var gi2 = g.NavBar.NavPaneForm.ExpandedGroupInfo;

                                                        rect = useText
                                                           ? li2.CaptionRectangle
                                                           : li2.ImageRectangle;

                                                        if (gi2 is not null && rect != Rectangle.Empty)
                                                        {
                                                            var navBarLiBadge = (Badge)liB.Clone();
                                                            navBarLiBadge.TargetElement = g.NavBar.NavPaneForm;
                                                            navPaneFormAdorner.Elements.Add(navBarLiBadge);

                                                            if (!gi2.ClientInfoBounds.Contains(li2.Bounds) && !gi2.ClientInfoBounds.IntersectsWith(li2.Bounds))
                                                            {
                                                                navBarLiBadge.Visible = false;
                                                            }
                                                            else
                                                            {
                                                                navBarLiBadge.Visible = true;
                                                            }
                                                            if (navBarLiBadge.Visible)
                                                            {
                                                                var piBadge = typeof(Badge).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                                                                if (piBadge.GetValue(navBarLiBadge, null) is BadgeViewInfo viBadge)
                                                                {
                                                                    if (needCalc || viBadge.Cache == null)
                                                                    {
                                                                        using (gi2.Graphics = navBarControl.CreateGraphics())
                                                                        {
                                                                            viBadge.Calc(gi2.Cache, gi2.ClientInfoBounds);
                                                                        }
                                                                    }
                                                                    var height = viBadge.Bounds.Height;
                                                                    navBarLiBadge.Properties.Offset = new Point(rect.Right, rect.Top + height / 2);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (rect != Rectangle.Empty)
                                                {
                                                    var gi = vi2.GetGroupInfo(g);
                                                    if (!gi.ClientInfoBounds.Contains(li.Bounds) && !gi.ClientInfoBounds.IntersectsWith(li.Bounds))
                                                    {
                                                        liB.Visible = false;
                                                    }
                                                    if (liB.Visible)
                                                    {
                                                        var piBadge = typeof(Badge).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                                                        if (piBadge.GetValue(liB, null) is BadgeViewInfo viBadge)
                                                        {
                                                            if (needCalc || viBadge.Cache == null)
                                                            {
                                                                using (gi.Graphics = navBarControl.CreateGraphics())
                                                                {
                                                                    viBadge.Calc(gi.Cache, gi.ClientInfoBounds);
                                                                }
                                                            }
                                                            var height = viBadge.Bounds.Height;
                                                            liB.Properties.Offset = new Point(rect.Right, rect.Top + height / 2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

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
            else if (e.Control is TreeList treeList)
            {
                // Customize TreeList in old templates and new templates with UseLightStyle set to false
            }
        }
        protected override void OnDeactivated()
        {
            disposables.Dispose();

            var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            if (showNavigationItemController is not null)
            {
                showNavigationItemController.ShowNavigationItemAction.CustomizeControl -= ShowNavigationItemAction_CustomizeControl;
            }

            base.OnDeactivated();
        }
    }
}
