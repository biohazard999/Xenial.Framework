using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
