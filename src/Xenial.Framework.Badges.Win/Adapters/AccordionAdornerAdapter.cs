using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
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

            if (accordionControl.GetViewInfo() is AccordionControlViewInfo accordionControlViewInfo)
            {
                foreach (var (choiceActionItem, accordionElement) in accordionElementCollection)
                {
                    if (accordionControlViewInfo.GetElementInfo(accordionElement) is AccordionElementBaseViewInfo accordionElementBaseViewInfo)
                    {
                        if (BadgeCollection.TryGetValue(choiceActionItem, out var badge))
                        {
                            badge.Properties.Offset = accordionElementBaseViewInfo.TextBounds.Location;
                        }
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
