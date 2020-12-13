using System;
using System.Collections.Generic;
using System.Linq;
using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;

using Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders;
using Xenial.Framework;
using Xenial.Framework.Base;
using Xenial.Framework.ModelBuilders;
using Bogus;
using System.ComponentModel;

namespace Xenial.FeatureCenter.Module
{
    public class FeatureCenterModule : XenialModuleBase
    {
        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
            .Concat(new[]
            {
                typeof(ModelBuilderBasicPropertiesDemo),

                typeof(WebViewEditorDemo),
                typeof(TokenEditorDemo),
                typeof(TokenEditorDemoTokens)
            });

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => base.GetDeclaredControllerTypes()
                .UseSingletonController();

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            updaters.UseSingletonNavigationItems();
            base.AddGeneratorUpdaters(updaters);
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
                        if (e.ObjectType == typeof(TokenEditorDemoTokens))
                        {
                            var faker = new Faker<TokenEditorDemoTokens>()
                                .RuleFor(r => r.Name, f => f.Name.FirstName());
                            var tokens = faker.Generate(100);
                            var bindingList = new BindingList<TokenEditorDemoTokens>();
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
        }
    }
}
