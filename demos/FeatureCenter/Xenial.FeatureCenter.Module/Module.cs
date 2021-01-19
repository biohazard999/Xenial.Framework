using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;

using Xenial.FeatureCenter.Module.BusinessObjects;
using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders;
using Xenial.FeatureCenter.Module.Model.GeneratorUpdaters;
using Xenial.FeatureCenter.Module.Updaters;

using Xenial.Framework;
using Xenial.Framework.StepProgressEditors;
using Xenial.Framework.TokenEditors;
using Xenial.Framework.WebView;

namespace Xenial.FeatureCenter.Module
{
    public sealed class FeatureCenterModule : XenialModuleBase
    {
        public static AvailablePlatform? CurrentPlatform { get; set; }

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(
                    typeof(ConditionalAppearanceModule),
                    typeof(XenialWebViewModule),
                    typeof(XenialTokenEditorsModule),
                    typeof(XenialStepProgressEditorsModule)
                );

        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
            .Concat(ModelTypeList.NonPersistentTypes)
            .Concat(ModelTypeList.PersistentTypes);

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => base.GetDeclaredControllerTypes()
                .UseSingletonController();

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
            => base.GetModuleUpdaters(objectSpace, versionFromDB)
                .Concat(new ModuleUpdater[]
                {
                    new SeedModuleUpdater(objectSpace, versionFromDB)
                });

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.Add(new FeatureCenterNavigationItemNodesUpdater());

            updaters.UseSingletonNavigationItems();
            updaters.UseNoViewsGeneratorUpdater();
            updaters.UseDetailViewLayoutBuilders();
        }

        public override void Setup(XafApplication application)
        {
            application.UseNonPersistentSingletons();

            base.Setup(application);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            typesInfo
                .CreateModelBuilder<ModelBuilderIntroductionDemoBuilder>()
                .Build();

            typesInfo
                .CreateModelBuilder<StepProgressBarEnumEditorDemoModelBuilder>()
                .Build();

            typesInfo
                .CreateModelBuilder<TokenStringEditorDemoModelBuilder>()
                .Build();

            typesInfo
                .CreateModelBuilder<TokenObjectsEditorDemoModelBuilder>()
                .Build();

            typesInfo
                .CreateModelBuilder<WebViewUriEditorDemoModelBuilder>()
                .Build();

            typesInfo
                .CreateModelBuilder<WebViewHtmlStringEditorDemoModelBuilder>()
                .Build();
        }
    }
}
