﻿using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

using Xenial.FeatureCenter.Module.BusinessObjects;
using Xenial.Framework;
using Xenial.Framework.Badges.Blazor;
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
                    typeof(XenialWebViewBlazorModule),
                    typeof(XenialBadgesBlazorModule)
                );

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => base.GetDeclaredControllerTypes().Concat(new[]
            {
                typeof(DownloadWindowsFormsDemoWindowController),
                typeof(HelpAndFeedbackWindowControllerBlazor)
            });

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            _ = application ?? throw new ArgumentNullException(nameof(application));
            application.CustomizeTemplate -= Application_CustomizeTemplate;
            application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        private void Application_CustomizeTemplate(object? sender, CustomizeTemplateEventArgs e)
        {
            if (e.Context == TemplateContext.ApplicationWindow)
            {
                AboutInfo.Instance.AboutInfoString = string.Join("<br>", FeatureCenterModule.VersionInformation);
            }
        }
    }
}
