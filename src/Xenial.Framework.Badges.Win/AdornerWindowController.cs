
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.XtraTreeList;

using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xenial.FeatureCenter.Module.Win
{
    public class AdornerWindowsFormsWindowController : WindowController
    {
        public AdornerWindowsFormsWindowController()
              => TargetWindowType = WindowType.Main;

        public AdornerUIManager? AdornerUIManager { get; set; }

        protected override void OnActivated()
        {
            base.OnActivated();
            AdornerUIManager = new AdornerUIManager();
            Frame.TemplateChanged -= Frame_TemplateChanged;
            Frame.TemplateChanged += Frame_TemplateChanged;
        }

        private void Frame_TemplateChanged(object sender, EventArgs e)
        {
            if (
                Frame.Template is System.Windows.Forms.ContainerControl containerControl
                && AdornerUIManager is not null
            )
            {
                AdornerUIManager.Owner = containerControl;
            }
        }

        protected override void OnDeactivated()
        {
            if (Frame is not null)
            {
                Frame.TemplateChanged -= Frame_TemplateChanged;
            }
            AdornerUIManager?.Dispose();
            base.OnDeactivated();
        }

        public void AddBadge(Badge badge)
        {
            if (AdornerUIManager is not null)
            {
                AdornerUIManager.Elements.Add(badge);
                AdornerUIManager.Update();
                AdornerUIManager.Show();
            }
        }
    }

    public class AdornerWindowsFormsCustomizeNavigationController : WindowController
    {
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
            if (container != null && container.Controls.Count == 1 && container.Controls[0] is TreeList treeList)
            {
                return treeList;
            }
            return null;
        }

        private void CustomizeEmbeddedTreeList(AdornerUIManager adorner, NavGroupCollection groups)
        {
            foreach (NavBarGroup group in groups)
            {
                var treeList = FindEmbeddedTreeList(group.ControlContainer);
                // Customize TreeList

                foreach (NavBarItem item in group.NavBar.Items)
                {
                    var badge = new Badge()
                    {
                        Visible = true,
                        Properties =
                        {
                            Text = "FOO",
                            PaintStyle = BadgePaintStyle.Information
                        }
                    };
                    badge.Appearance.BackColor = Color.LightCyan;
                    badge.Visible = true;
                    badge.Properties.Location = ContentAlignment.MiddleCenter;

                    badge.TargetElement = group.NavBar;
                    adorner.Elements.Add(badge);
                    adorner.Show();
                    adorner.Update();
                    //adornerController.AddBadge(badge);
                }
            }
        }

        private void ShowNavigationItemAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            if (e.Control is DevExpress.XtraBars.BarButtonItem barButtonItem)
            {

            }
            if (e.Control is AccordionControl accordionControl)
            {
                // Customize AccordionControl
            }
            else if (e.Control is NavBarControl navBarControl)
            {
                var containerControl = FindContainerControl(navBarControl);

                ContainerControl? FindContainerControl(Control? control)
                {
                    if (control is ContainerControl container)
                    {
                        return container;
                    }
                    return FindContainerControl(control?.Parent);
                }
                if (containerControl is not null)
                {
                    navBarControl.MouseMove += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            UpdateBadges(navBarControl, true);
                        }
                    };

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

                    var adorner = new AdornerUIManager();
                    adorner.Owner = containerControl;

                    var badge = new Badge()
                    {
                        Visible = true,
                        Properties =
                        {
                            Text = "FOO",
                            PaintStyle = BadgePaintStyle.Information
                        }
                    };
                    badge.Appearance.BackColor = Color.LawnGreen;
                    badge.Visible = true;
                    badge.Properties.Location = ContentAlignment.MiddleCenter;
                    badge.Appearance.BackColor = System.Drawing.Color.Yellow;
                    badge.Appearance.Options.UseBackColor = true;
                    badge.TargetElement = navBarControl;

                    adorner.Elements.Add(badge);
                    //adorner.Show();
                    //adorner.Update();
                    // Customize NavBarControl
                    CustomizeEmbeddedTreeList(adorner, navBarControl.Groups);
                }
            }
            else if (e.Control is TreeList treeList)
            {
                // Customize TreeList in old templates and new templates with UseLightStyle set to false
            }
        }
        protected override void OnDeactivated()
        {
            var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            if (showNavigationItemController is not null)
            {
                showNavigationItemController.ShowNavigationItemAction.CustomizeControl -= ShowNavigationItemAction_CustomizeControl;
            }

            base.OnDeactivated();
        }
    }
}
