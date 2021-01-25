using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation.Win;

using Xenial.Framework;

namespace MailClient.Module.Win
{
    public class MailClientWindowsFormsModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore().AndModuleTypes(new[]
            {
                typeof(ValidationWindowsFormsModule),
                typeof(MailClientModule)
            });
    }
}
