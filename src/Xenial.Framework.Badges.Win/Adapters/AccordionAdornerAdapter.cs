using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils.Drawing;
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
        private readonly Dictionary<ChoiceActionItem, AccordionControlElement> popupAccordionElementCollection = new();

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

            void MouseMove(object? sender, MouseEventArgs e)
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

            void NeedInvoke(object? sender, EventArgs e)
                => BeginInvokeUpdateBadges();

            BeginInvokeUpdateBadges();
        }

        private void BeginInvokeUpdateBadges()
            => BeginInvokeAction(() => UpdateBadges());

        private void UpdateBadges(bool needCalc = false)
        {
            AdornerUIManager.BeginUpdate();
            try
            {
                UpdatePopup();

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
                                        using var graphics = accordionControl.CreateGraphics();
                                        using var cache = new GraphicsCache(new DXPaintEventArgs(graphics));
                                        badgeViewInfo.Calc(cache, rect);
                                    }

                                    var height = badgeViewInfo.Bounds.Height;
                                    var width = badgeViewInfo.Bounds.Width;
                                    badge.Properties.Offset = new Point(rect.Right + width / 2, rect.Top + rect.Height / 2);
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

        private void UpdatePopup()
        {
            var popupForm = accordionControl.GetPopupForm();

            if (popupForm is not null)
            {
                if (!accordionControl.IsPopupFormShown)
                {
                    AttachToPopupEvents(popupForm);
                }
            }
        }

        private void AttachToPopupEvents(AccordionControlForm popupForm)
        {
            if (!accordionControlFormCollection.TryGetValue(popupForm, out var adornerPopupMgr))
            {
                adornerPopupMgr = new AdornerUIManager();
                adornerPopupMgr.Owner = popupForm;
                accordionControlFormCollection[popupForm] = adornerPopupMgr;

                Dispose(() => popupForm.Disposed -= PopupFormDisposed);
                popupForm.Disposed -= PopupFormDisposed;
                popupForm.Disposed += PopupFormDisposed;
                void PopupFormDisposed(object? sender, EventArgs e)
                {
                    popupForm.Disposed -= PopupFormDisposed;
                    accordionControlFormCollection.Remove(popupForm);
                }
            }

            Dispose(() => popupForm.Shown -= PopupForm_Shown);
            popupForm.Shown -= PopupForm_Shown;
            popupForm.Shown += PopupForm_Shown;

            Dispose(() => popupForm.Activated -= PopupForm_Shown);
            popupForm.Activated -= PopupForm_Shown;
            popupForm.Activated += PopupForm_Shown;

            Dispose(() => popupForm.Deactivate -= PopupForm_Deactivate);
            popupForm.Deactivate -= PopupForm_Deactivate;
            popupForm.Deactivate += PopupForm_Deactivate;

            Dispose(() => popupForm.FormClosed -= PopupForm_Deactivate);
            popupForm.FormClosed -= PopupForm_Deactivate;
            popupForm.FormClosed += PopupForm_Deactivate;
        }

        private void PopupForm_Shown(object? sender, EventArgs e)
        {
            if (sender is AccordionControlForm popupForm)
            {
                var popupAccordion = popupForm.Controls.OfType<AccordionControl>().FirstOrDefault();

                if (
                    popupAccordion is not null
                    && accordionControlFormCollection.TryGetValue(popupForm, out var adornerPopupManager)
                )
                {
                    Dispose(() => popupAccordion.FilterContent -= FilterPopupAccordion);
                    popupAccordion.FilterContent -= FilterPopupAccordion;
                    popupAccordion.FilterContent += FilterPopupAccordion;
                    Dispose(() => accordionControl.ExpandStateChanged -= FilterPopupAccordion);
                    popupAccordion.ExpandStateChanged -= FilterPopupAccordion;
                    popupAccordion.ExpandStateChanged += FilterPopupAccordion;

                    void FilterPopupAccordion(object sender, EventArgs e)
                        => BeginInvokeAction(() =>
                        {
                            adornerPopupManager.BeginUpdate();
                            try
                            {
                                adornerPopupManager.Elements.Clear();
                            }
                            finally
                            {
                                adornerPopupManager.EndUpdate();
                            }
                            UpdatePopupBadges(popupAccordion, adornerPopupManager);
                        });

                    UpdatePopupBadges(popupAccordion, adornerPopupManager);
                }
            }
        }

        private void UpdatePopupBadges(AccordionControl popupAccordion, AdornerUIManager adornerPopupManager)
        {
            adornerPopupManager.BeginUpdate();
            try
            {
                CollectPopupElements(popupAccordion.Elements);

                foreach (var (choiceActionItem, popupAccordionControlElement) in popupAccordionElementCollection)
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
                                popupBadge.Visible = popupAccordionControlElement.IsVisible;
                                var badgeViewInfo = popupBadge.GetViewInfo();

                                if (badgeViewInfo is not null)
                                {
                                    if (badgeViewInfo.Cache is null)
                                    {
                                        using var graphics = accordionControl.CreateGraphics();
                                        using var cache = new GraphicsCache(new DXPaintEventArgs(graphics));
                                        badgeViewInfo.Calc(cache, popupRect);
                                    }

                                    var width = badgeViewInfo.Bounds.Width;

                                    popupBadge.Properties.Offset = new Point(popupRect.Right + width / 2, popupRect.Top + popupRect.Height / 2);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                adornerPopupManager.EndUpdate();
            }
        }

        private void CollectPopupElements(AccordionControlElementCollection elements)
        {
            foreach (var element in elements)
            {
                if (element.Tag is ChoiceActionItem choiceActionItem)
                {
                    if (BadgeCollection.ContainsKey(choiceActionItem))
                    {
                        popupAccordionElementCollection[choiceActionItem] = element;
                    }
                }
                CollectPopupElements(element.Elements);
            }
        }

        private void PopupForm_Deactivate(object? sender, EventArgs e)
        {
            popupAccordionElementCollection.Clear();
            if (sender is AccordionControlForm popupForm)
            {
                if (accordionControlFormCollection.TryGetValue(popupForm, out var popupAdornerUIManager))
                {
                    popupAdornerUIManager.BeginUpdate();
                    try
                    {
                        popupAdornerUIManager.Elements.Clear();
                    }
                    finally
                    {
                        popupAdornerUIManager.EndUpdate();
                    }
                }
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
