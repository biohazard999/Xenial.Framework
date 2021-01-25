using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation.Blazor;

using Xenial.Framework;

namespace MailClient.Module.Blazor
{
    public class MailClientBlazorModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore().AndModuleTypes(new[]
            {
                typeof(ValidationBlazorModule),
                typeof(MailClientModule)
            });
    }
}
