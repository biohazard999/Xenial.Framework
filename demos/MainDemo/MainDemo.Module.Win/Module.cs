using System;

using DevExpress.ExpressApp;

using Xenial.Framework;

namespace MainDemo.Module.Win
{
    public sealed class MainDemoWinModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore() => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(new[]
            {
                typeof(MainDemoModule)
            });
    }
}
