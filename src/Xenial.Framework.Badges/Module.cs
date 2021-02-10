using System;
using System.Collections.Generic;

using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Badges
{
    public class XenialBadgesModule : XenialModuleBase
    {
        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
                .UseBadgesRegularTypes();

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.UseNavigationItemBadges();
        }
    }
}
