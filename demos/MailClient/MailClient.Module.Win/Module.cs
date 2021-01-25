using System;

using DevExpress.ExpressApp;

using Xenial.Framework;

namespace MailClient.Module.Win
{
    public class MailClientWindowsFormsModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore().AndModuleTypes(new[]
            {
                typeof(MailClientModule)
            });
    }
}
