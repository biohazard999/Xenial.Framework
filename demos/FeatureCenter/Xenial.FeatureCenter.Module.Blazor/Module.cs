using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.Framework;
using Xenial.Framework.TokenEditors.Blazor;

namespace Xenial.FeatureCenter.Module.Blazor
{
    public sealed class FeatureCenterBlazorModule : XenialModuleBase
    {
        public FeatureCenterBlazorModule() => FeatureCenterModule.CurrentPlatform = BusinessObjects.AvailablePlatform.Blazor;

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(
                    typeof(FeatureCenterModule),
                    typeof(XenialTokenEditorsBlazorModule)
                );
    }
}
