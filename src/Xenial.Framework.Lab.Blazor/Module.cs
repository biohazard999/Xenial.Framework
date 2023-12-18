using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.SystemModule;

namespace Xenial.Framework.Lab.Blazor
{
    [XenialCheckLicense]
    public sealed partial class XenialLabBlazorModule : XenialModuleBase
    {
        /// <summary>   Gets required module types core. </summary>
        ///
        /// <returns>   The required module types core. </returns>

        protected override ModuleTypeList GetRequiredModuleTypesCore() => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(new[]
            {
                typeof(SystemBlazorModule),
                typeof(XenialLabModule)
            });
    }
}
