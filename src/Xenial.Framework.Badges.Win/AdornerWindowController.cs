using System;
using System.Collections.Generic;
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
                adornerAdapters.Add(adapter);
                disposables.Add(adapter);
                var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
                if (showNavigationItemController is not null)
                {
                    adapter.Enable(showNavigationItemController);
                    return;
                };
            }
            else if (e.Control is TreeList treeList)
            {
                // Customize TreeList in old templates and new templates with UseLightStyle set to false
            }
        }
        protected override void OnDeactivated()
        {
            foreach (var adornerAdapter in adornerAdapters)
            {
                adornerAdapter.Disable();
            }

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
