using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Badges.Model;

namespace Xenial.FeatureCenter.Module
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class FeatureStatusAttribute : Attribute
    {
        public string BadgeText { get; }
        public XenialStaticBadgePaintStyle BadgePaintStyle { get; }
        public FeatureStatusAttribute(string badgeText, XenialStaticBadgePaintStyle badgePaintStyle)
            => (BadgeText, BadgePaintStyle) = (badgeText, badgePaintStyle);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class FeatureStatusAlphaAttribute : FeatureStatusAttribute
    {
        public FeatureStatusAlphaAttribute()
            : base("Alpha", XenialStaticBadgePaintStyle.System) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class FeatureStatusBetaAttribute : FeatureStatusAttribute
    {
        public FeatureStatusBetaAttribute()
            : base("Beta", XenialStaticBadgePaintStyle.Question) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class FeatureStatusStableAttribute : FeatureStatusAttribute
    {
        public FeatureStatusStableAttribute()
            : base("Stable", XenialStaticBadgePaintStyle.Information) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class FeatureStatusLabAttribute : FeatureStatusAttribute
    {
        public FeatureStatusLabAttribute()
            : base("Lab", XenialStaticBadgePaintStyle.Critical) { }
    }
}
