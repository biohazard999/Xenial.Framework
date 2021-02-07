
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraNavBar;
using DevExpress.XtraTreeList;

using System;
using System.Drawing;
using System.Linq;
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

                    badge.TargetElement = item;
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
