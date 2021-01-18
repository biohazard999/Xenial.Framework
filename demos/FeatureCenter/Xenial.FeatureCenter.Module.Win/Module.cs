using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;

using Xenial.Framework;
using Xenial.Framework.StepProgressEditors.Win;
using Xenial.Framework.TokenEditors.Win;
using Xenial.Framework.WebView.Win;

namespace Xenial.FeatureCenter.Module.Win
{
    public sealed class FeatureCenterWindowsFormsModule : XenialModuleBase
    {
        public FeatureCenterWindowsFormsModule() => FeatureCenterModule.CurrentPlatform = BusinessObjects.AvailablePlatform.Win;

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(
                typeof(FeatureCenterModule),
                typeof(XenialWebViewWindowsFormsModule),

                typeof(XenialTokenEditorsWindowsFormsModule),
                typeof(XenialStepProgressEditorsWindowsFormsModule)
            );
    }
}
