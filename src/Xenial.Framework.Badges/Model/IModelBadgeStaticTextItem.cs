using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Framework.Badges.Model;

namespace Xenial.Framework.Badges.Model
{
    public interface IXenialModelBadgeStaticTextItem
    {
        [Category("Xenial.Badges")]
        [ModelPersistentName("Xenial.Badges." + nameof(XenialBadgeStaticText))]
        [Description("Gets or sets a static text badge on the item")]
        [DisplayName("StaticText")]
        string? XenialBadgeStaticText { get; set; }
    }
}



namespace DevExpress.ExpressApp.Model
{
    public static partial class ModelInterfaceExtendersBadgesExtensions
    {
        public static ModelInterfaceExtenders UseNavigationItemBadges(this ModelInterfaceExtenders extenders)
        {
            _ = extenders ?? throw new ArgumentNullException(nameof(extenders));

            extenders.Add<IModelNavigationItem, IXenialModelBadgeStaticTextItem>();

            return extenders;
        }
    }
}

namespace DevExpress.ExpressApp.Model
{
    public static partial class XenialBadgesModelTypeList
    {
        public static IEnumerable<Type> UseBadgesRegularTypes(this IEnumerable<Type> types)
            => types.Concat(new[]
            {
                typeof(IXenialModelBadgeStaticTextItem)
            });
    }
}
