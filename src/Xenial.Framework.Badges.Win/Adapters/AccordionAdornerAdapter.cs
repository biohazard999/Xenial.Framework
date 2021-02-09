using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils.DPI;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars.Navigation;

using Xenial.Framework.Badges.Win.Helpers;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal class AccordionAdornerAdapter : AdornerAdapterBase
    {
        private readonly AccordionControl accordionControl;
        private readonly Dictionary<ChoiceActionItem, AccordionControlElement> accordionElementCollection = new();
        private readonly Dictionary<AccordionControlForm, AdornerUIManager> accordionControlFormCollection = new();
        protected override Control DefaultTargetElement => accordionControl;

        internal AccordionAdornerAdapter(AccordionControl accordionControl) : base(new AdornerUIManager())
            => this.accordionControl = accordionControl;

        public override void Enable(ShowNavigationItemController showNavigationItemController)
        {
            base.Enable(showNavigationItemController);
            CollectElements(accordionControl.Elements);
            AttachToEvents();
        }

        private void AttachToEvents()
        {
            Dispose(() => accordionControl.MouseMove -= MouseMove);
            accordionControl.MouseMove -= MouseMove;
            accordionControl.MouseMove += MouseMove;

            Dispose(() => accordionControl.MouseUp -= NeedInvoke);
            accordionControl.MouseUp -= NeedInvoke;
            accordionControl.MouseUp += NeedInvoke;

            void MouseMove(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    UpdateBadges(true);
                }
            }

            Dispose(() => accordionControl.FilterContent -= NeedInvoke);
            accordionControl.FilterContent -= NeedInvoke;
            accordionControl.FilterContent += NeedInvoke;

            Dispose(() => accordionControl.ExpandStateChanged -= NeedInvoke);
            accordionControl.ExpandStateChanged -= NeedInvoke;
            accordionControl.ExpandStateChanged += NeedInvoke;

            Dispose(() => accordionControl.Resize -= NeedInvoke);
            accordionControl.Resize -= NeedInvoke;
            accordionControl.Resize += NeedInvoke;

            void NeedInvoke(object sender, EventArgs e)
                => BeginInvokeUpdateBadges();
        }

        private void BeginInvokeUpdateBadges()
            => BeginInvokeAction(() => UpdateBadges());

        private void UpdateBadges(bool needCalc = false)
        {
            AdornerUIManager.BeginUpdate();
            try
            {
                if (accordionControl.GetViewInfo() is AccordionControlViewInfo accordionControlViewInfo)
                {
                    foreach (var (choiceActionItem, accordionElement) in accordionElementCollection)
                    {
                        if (accordionControlViewInfo.GetElementInfo(accordionElement) is AccordionElementBaseViewInfo accordionElementBaseViewInfo)
                        {
                            if (BadgeCollection.TryGetValue(choiceActionItem, out var badge))
                            {
                                var badgeViewInfo = badge.GetViewInfo();
                                if (badgeViewInfo is not null)
                                {
                                    badge.Visible = accordionElement.IsVisible
                                        && accordionControl.OptionsMinimizing.State == AccordionControlState.Normal;

                                    if (
                                        accordionElement.Level == 0
                                        && accordionControl.OptionsMinimizing.State == AccordionControlState.Minimized
                                    )
                                    {
                                        badge.Visible = true;
                                    }

                                    var rect = accordionElementBaseViewInfo.TextBounds;
                                    if (needCalc || badgeViewInfo.Cache is null)
                                    {
                                        using (var graphics = accordionControl.CreateGraphics())
                                        {
                                            badgeViewInfo.Calc(new GraphicsCache(new DXPaintEventArgs(graphics)), rect);
                                        }
                                    }

                                    var height = badgeViewInfo.Bounds.Height;
                                    var width = badgeViewInfo.Bounds.Width;
                                    badge.Properties.Offset = new Point(rect.Right + width / 2, rect.Top + rect.Height / 2);

                                    var popupForm = accordionControl.GetPopupForm();

                                    if (popupForm is not null)
                                    {
                                        if (!accordionControl.IsPopupFormShown)
                                        {
                                            //popupForm.Shown -= Shown;
                                            popupForm.Shown += Shown;
                                            popupForm.Activated += Shown;
                                            popupForm.Deactivate += Deactivate;

                                            void Shown(object sender, EventArgs e)
                                            {
                                                DoShit();
                                            }

                                            void Deactivate(object sender, EventArgs e)
                                            {

                                            }
                                        }
                                        else
                                        {
                                            DoShit();
                                        }
                                        void DoShit()
                                        {
                                            if (popupForm is null)
                                            {
                                                return;
                                            }

                                            if (!accordionControlFormCollection.TryGetValue(popupForm, out var adornerPopupMgr) && adornerPopupMgr is null)
                                            {
                                                adornerPopupMgr = new AdornerUIManager();
                                                adornerPopupMgr.Owner = popupForm;
                                                accordionControlFormCollection[popupForm] = adornerPopupMgr;

                                                popupForm.Disposed -= PopupFormDisposed;
                                                popupForm.Disposed += PopupFormDisposed;
                                                void PopupFormDisposed(object sender, EventArgs e)
                                                {
                                                    popupForm.Disposed -= PopupFormDisposed;
                                                    accordionControlFormCollection.Remove(popupForm);
                                                }
                                            }
                                            if (accordionControlFormCollection.TryGetValue(popupForm, out var adornerPopupManager))
                                            {
                                                var popupAccordion = popupForm.Controls.OfType<AccordionControl>().FirstOrDefault();

                                                var popupAccordionElementCollection = new Dictionary<ChoiceActionItem, AccordionControlElement>();

                                                CollectPopupElements(popupAccordionElementCollection, popupAccordion.Elements);
                                                void CollectPopupElements(
                                                    Dictionary<ChoiceActionItem, AccordionControlElement> collection,
                                                    AccordionControlElementCollection elements
                                                    )
                                                {
                                                    foreach (var element in elements)
                                                    {
                                                        if (element.Tag is ChoiceActionItem choiceActionItem)
                                                        {
                                                            if (BadgeCollection.ContainsKey(choiceActionItem))
                                                            {
                                                                collection[choiceActionItem] = element;
                                                            }
                                                        }
                                                        CollectPopupElements(collection, element.Elements);
                                                    }
                                                }
                                                if (popupAccordionElementCollection.TryGetValue(choiceActionItem, out var popupAccordionControlElement))
                                                {
                                                    if (popupAccordion.GetViewInfo() is AccordionControlViewInfo popupAccordionControlViewInfo)
                                                    {
                                                        if (BadgeCollection.TryGetValue(choiceActionItem, out var badge))
                                                        {
                                                            var elViewInfo = popupAccordionControlViewInfo.GetElementInfo(popupAccordionControlElement);
                                                            if (elViewInfo is not null)
                                                            {
                                                                var popupBadge = (Badge)badge.Clone();
                                                                popupBadge.TargetElement = popupAccordion;
                                                                adornerPopupManager.Elements.Add(popupBadge);
                                                                var popupRect = elViewInfo.TextBounds;
                                                                popupBadge.Visible = true;
                                                                popupBadge.Properties.Offset = new Point(popupRect.Right + width / 2, popupRect.Top + popupRect.Height / 2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                AdornerUIManager.EndUpdate();
            }
        }


        private void CollectElements(AccordionControlElementCollection elements)
        {
            foreach (var element in elements)
            {
                if (element.Tag is ChoiceActionItem choiceActionItem)
                {
                    if (BadgeCollection.ContainsKey(choiceActionItem))
                    {
                        accordionElementCollection[choiceActionItem] = element;
                    }
                }
                CollectElements(element.Elements);
            }
        }
    }
}
