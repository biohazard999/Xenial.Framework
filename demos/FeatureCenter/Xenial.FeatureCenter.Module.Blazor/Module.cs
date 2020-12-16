using System;

using DevExpress.ExpressApp;

using Xenial.Framework;
using Xenial.Framework.TokenEditors.Blazor;

namespace Xenial.FeatureCenter.Module.Blazor
{
    public class FeatureCenterBlazorModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(
                    typeof(FeatureCenterModule),
                    typeof(XenialTokenEditorsBlazorModule)
                );
    }
}
