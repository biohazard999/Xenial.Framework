using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.Framework;
using Xenial.Framework.TokenEditors.Blazor;
using Xenial.Framework.WebView.Blazor;

namespace Xenial.FeatureCenter.Module.Blazor
{
    public sealed class FeatureCenterBlazorModule : XenialModuleBase
    {
        public FeatureCenterBlazorModule() => FeatureCenterModule.CurrentPlatform = AvailablePlatform.Blazor;

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(
                    typeof(FeatureCenterModule),
                    typeof(XenialTokenEditorsBlazorModule),
                    typeof(XenialWebViewBlazorModule)
                );

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => base.GetDeclaredControllerTypes().Concat(new[] { typeof(DownloadWindowsFormsDemoWindowController) });
    }
}
