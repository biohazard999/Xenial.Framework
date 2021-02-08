using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Actions;
using DevExpress.Utils.VisualEffects;

using Xenial.Framework.Badges.Model;
using Xenial.Framework.Badges.Win.Helpers;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal static class ActionItemBadgeFactory
    {
        internal static Badge? CreateBadge(ChoiceActionItem choiceActionItem)
        {
            if (
                choiceActionItem.Model is IXenialModelBadgeStaticTextItem modelBadgeStaticTextItem
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
                return badge;
            }
            return null;
        }
    }
}
