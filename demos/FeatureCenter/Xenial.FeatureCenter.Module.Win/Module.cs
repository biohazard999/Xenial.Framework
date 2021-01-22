using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.Framework;
using Xenial.Framework.StepProgressEditors.Win;
using Xenial.Framework.TokenEditors.Win;
using Xenial.Framework.WebView.Win;

namespace Xenial.FeatureCenter.Module.Win
{
    public sealed class FeatureCenterWindowsFormsModule : XenialModuleBase
    {
        public FeatureCenterWindowsFormsModule() => FeatureCenterModule.CurrentPlatform = AvailablePlatform.Win;

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(
                typeof(FeatureCenterModule),
                typeof(XenialWebViewWindowsFormsModule),

                typeof(XenialTokenEditorsWindowsFormsModule),
                typeof(XenialStepProgressEditorsWindowsFormsModule)
            );

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => base.GetDeclaredControllerTypes().Concat(new[]
            {
                typeof(OpenBlazorDemoWindowController),
                typeof(StatusBarVersionWindowController)
            });
    }
}
