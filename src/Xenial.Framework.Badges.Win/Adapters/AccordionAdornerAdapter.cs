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
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraBars.Navigation;

using Xenial.Framework.Badges.Win.Helpers;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal class AccordionAdornerAdapter : AdornerAdapterBase
    {
        private readonly AccordionControl accordionControl;
        private readonly Dictionary<ChoiceActionItem, AccordionControlElement> accordionElementCollection = new();
        protected override Control DefaultTargetElement => accordionControl;

        internal AccordionAdornerAdapter(AccordionControl accordionControl) : base(new AdornerUIManager())
            => this.accordionControl = accordionControl;

        public override void Enable(ShowNavigationItemController showNavigationItemController)
        {
            base.Enable(showNavigationItemController);
            CollectElements(accordionControl.Elements);

            foreach (var (choiceActionItem, badge) in BadgeCollection)
            {
                badge.Visible = true;
            }

            AttachToEvents();
        }

        private void AttachToEvents()
        {
            Dispose(() => accordionControl.MouseMove -= MouseMove);
            accordionControl.MouseMove -= MouseMove;
            accordionControl.MouseMove += MouseMove;

            void MouseMove(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    UpdateBadges(true);
                }
            }

            //Dispose(() => accordionControl.GroupCollapsed -= GroupChange);
            //accordionControl.GroupCollapsed -= GroupChange;
            //accordionControl.GroupCollapsed += GroupChange;

            //Dispose(() => accordionControl.GroupExpanded -= GroupChange);
            //accordionControl.GroupExpanded -= GroupChange;
            //accordionControl.GroupExpanded += GroupChange;

            //void GroupChange(object sender, EventArgs e)
            //{
            //    UpdateBadges(true);
            //}

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
