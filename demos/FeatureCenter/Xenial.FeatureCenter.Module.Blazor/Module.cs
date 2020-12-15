using System;

using DevExpress.ExpressApp;

using Xenial.Framework;

namespace Xenial.FeatureCenter.Module.Blazor
{
    public class FeatureCenterBlazorModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(typeof(FeatureCenterModule));
    }
}
