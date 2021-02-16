using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xenial.Framework.Lab.Win
{
    [XenialCheckLicence]
    public sealed partial class XenialLabWindowsFormsModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore() => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(new[]
            {
                typeof(SystemWindowsFormsModule),
                typeof(XenialLabModule)
            });

        protected override IEnumerable<Type> GetDeclaredControllerTypes() => base.GetDeclaredControllerTypes().Concat(new[]
        {
            typeof(ExtendedLayoutController)
        });
    }
}
