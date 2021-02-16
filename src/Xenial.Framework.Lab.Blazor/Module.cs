using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.SystemModule;

namespace Xenial.Framework.Lab.Blazor
{
    [XenialCheckLicence]
    public sealed partial class XenialLabBlazorModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore() => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(new[]
            {
                typeof(SystemBlazorModule),
                typeof(XenialLabModule)
            });
    }
}
