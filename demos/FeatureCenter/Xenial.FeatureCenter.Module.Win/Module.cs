using System;

using DevExpress.ExpressApp;

using Xenial.Framework;

namespace Xenial.FeatureCenter.Module.Win
{
    public class FeatureCenterWindowsFormsModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(
                typeof(FeatureCenterModule)
            );
    }
}
