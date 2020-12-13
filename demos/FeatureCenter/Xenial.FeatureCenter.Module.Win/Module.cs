using System;

using DevExpress.ExpressApp;

using Xenial.Framework;
using Xenial.Framework.TokenEditors.Win;
using Xenial.Framework.WebView.Win;

namespace Xenial.FeatureCenter.Module.Win
{
    public class FeatureCenterWindowsFormsModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(
                typeof(FeatureCenterModule),
                typeof(XenialWebViewWindowsFormsModule),
                typeof(XenialTokenEditorsWindowsFormsModule)
            );
    }
}
