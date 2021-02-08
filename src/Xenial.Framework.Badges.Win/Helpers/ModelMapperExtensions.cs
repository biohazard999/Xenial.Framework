using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Utils.VisualEffects;

using Xenial.Framework.Badges.Model;

namespace Xenial.Framework.Badges.Win.Helpers
{
    internal static class ModelMapperExtensions
    {
        internal static BadgePaintStyle ConvertPaintStyle(this XenialStaticBadgePaintStyle? paintStyle)
            => paintStyle switch
            {
                XenialStaticBadgePaintStyle.Critical => BadgePaintStyle.Critical,
                XenialStaticBadgePaintStyle.Information => BadgePaintStyle.Information,
                XenialStaticBadgePaintStyle.Question => BadgePaintStyle.Question,
                XenialStaticBadgePaintStyle.System => BadgePaintStyle.System,
                XenialStaticBadgePaintStyle.Warning => BadgePaintStyle.Warning,
                _ => BadgePaintStyle.Default
            };

#if !NET5_0
        internal static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
            => (key, value) = (tuple.Key, tuple.Value);
#endif

        private delegate AdornerElementViewInfo? GetBadgeViewInfoDelegate(Badge badge);

        private static GetBadgeViewInfoDelegate? getBadgeViewInfo;

        private static readonly object locker = new();

        internal static BadgeViewInfo? GetViewInfo(this Badge badge)
        {
            if (getBadgeViewInfo is null)
            {
                var propertyInfo = typeof(Badge).GetProperty("ViewInfo", BindingFlags.Instance | BindingFlags.NonPublic);

                if (
                    propertyInfo is not null
                    && propertyInfo.GetMethod is MethodInfo getMethod
                )
                {
                    lock (locker)
                    {
                        getBadgeViewInfo = (GetBadgeViewInfoDelegate)Delegate.CreateDelegate(typeof(GetBadgeViewInfoDelegate), getMethod);
                    }
                }
            }

            if (getBadgeViewInfo is not null)
            {
                if (getBadgeViewInfo(badge) is BadgeViewInfo badgeViewInfo)
                {
                    return badgeViewInfo;
                }
            }

            return null;
        }
    }
}
