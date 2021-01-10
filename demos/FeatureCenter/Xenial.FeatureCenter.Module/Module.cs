using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using Bogus;

using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;

using Xenial.Framework;
using Xenial.Framework.StepProgressEditors;
using Xenial.Framework.TokenEditors;
using Xenial.Framework.WebView;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders;
using Xenial.FeatureCenter.Module.BusinessObjects;
using Xenial.FeatureCenter.Module.Updaters;

namespace Xenial.FeatureCenter.Module
{
    public class FeatureCenterModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(
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

            updaters.UseSingletonNavigationItems();
            updaters.UseDetailViewLayoutBuilders();
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);

            editorDescriptorsFactory.UseTokenObjectsPropertyEditors<TokenEditorNonPersistentTokens>();
            editorDescriptorsFactory.UseTokenObjectsPropertyEditorsForType<XPCollection<TokenEditorPersistentTokens>>();
        }

        public override void Setup(XafApplication application)
        {
            application.UseNonPersistentSingletons();

            application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
            application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
            application.Disposed -= Application_Disposed;
            application.Disposed += Application_Disposed;

            void Application_ObjectSpaceCreated(object? _, ObjectSpaceCreatedEventArgs e)
            {
                if (e.ObjectSpace is NonPersistentObjectSpace nos)
                {
                    nos.ObjectsGetting -= Nos_ObjectsGetting;
                    nos.ObjectsGetting += Nos_ObjectsGetting;
                    nos.Disposed -= Nos_Disposed;
                    nos.Disposed += Nos_Disposed;


                    void Nos_ObjectsGetting(object sender, ObjectsGettingEventArgs e)
                    {
                        if (e.ObjectType == typeof(TokenEditorNonPersistentTokens))
                        {
                            var faker = new Faker<TokenEditorNonPersistentTokens>()
                                .RuleFor(r => r.Name, f => f.Name.FirstName());
                            var tokens = faker.Generate(100);
                            var bindingList = new BindingList<TokenEditorNonPersistentTokens>();
                            foreach (var token in tokens)
                            {
                                bindingList.Add(token);
                            }
                            e.Objects = bindingList;
                        }
                    }

                    void Nos_Disposed(object? _, EventArgs e)
                    {
                        nos.Disposed -= Nos_Disposed;
                        nos.ObjectsGetting -= Nos_ObjectsGetting;
                    }
                }
            }

            void Application_Disposed(object? _, EventArgs e)
            {
                application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
                application.Disposed -= Application_Disposed;
            }

            base.Setup(application);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            typesInfo
                .CreateModelBuilder<ModelBuilderBasicPropertiesDemoModelBuilder>()
                .Build();

            typesInfo
                .CreateModelBuilder<WebViewEditorDemoModelBuilder>()
                .Build();

            typesInfo
                .CreateModelBuilder<StepProgressBarEnumEditorDemoModelBuilder>()
                .Build();
        }
    }
}
