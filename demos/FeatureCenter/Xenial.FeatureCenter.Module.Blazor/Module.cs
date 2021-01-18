using System;

using DevExpress.ExpressApp;

using Xenial.Framework;
using Xenial.Framework.TokenEditors.Blazor;
using Xenial.Framework.WebView.Blazor;

namespace Xenial.FeatureCenter.Module.Blazor
{
    public sealed class FeatureCenterBlazorModule : XenialModuleBase
    {
        public FeatureCenterBlazorModule() => FeatureCenterModule.CurrentPlatform = BusinessObjects.AvailablePlatform.Blazor;

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(
                    typeof(FeatureCenterModule),
                    typeof(XenialTokenEditorsBlazorModule),
                    typeof(XenialWebViewBlazorModule)
                );
    }
}
