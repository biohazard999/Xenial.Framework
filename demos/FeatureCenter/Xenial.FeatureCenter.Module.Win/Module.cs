﻿using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;

using Xenial.FeatureCenter.Module.BusinessObjects;
using Xenial.Framework;
using Xenial.Framework.Badges.Win;
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
                typeof(HelpAndFeedbackWindowControllerWin),
                typeof(StatusBarVersionWindowController),
                typeof(BadgedFeatureController),

                //TODO: CREATE MODULE
                typeof(AdornerWindowsFormsCustomizeNavigationController)
            });

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.UseApplicationWinOptions(new Framework.Win.Model.GeneratorUpdaters.ApplicationWinOptions
            {
                EnableHtmlFormatting = true,
                FormStyle = DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon,
                RibbonOptions =
                {
                    RibbonControlStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.OfficeUniversal
                }
            });
        }

        public override void CustomizeLogics(CustomLogics customLogics)
        {
            base.CustomizeLogics(customLogics);
            customLogics.UseUiType(UIType.TabbedMDI);
        }
    }
}
