using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;

using Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders;
using Xenial.Framework;
using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module
{
    public class FeatureCenterModule : XenialModuleBase
    {
        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
            .Concat(new[]
            {
                typeof(ModelBuilderBasicPropertiesDemo)
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
            application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
            application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
            application.Disposed -= Application_Disposed;
            application.Disposed += Application_Disposed;

            void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
            {
                if (e.ObjectSpace is NonPersistentObjectSpace nos)
                {
                    nos.ObjectByKeyGetting -= Nos_ObjectByKeyGetting;
                    nos.ObjectByKeyGetting += Nos_ObjectByKeyGetting;
                    nos.Disposed -= Nos_Disposed;
                    nos.Disposed += Nos_Disposed;

                    void Nos_ObjectByKeyGetting(object _, ObjectByKeyGettingEventArgs e1)
                    {
                        var typeInfo = application.TypesInfo.FindTypeInfo(e1.ObjectType);

                        if (typeInfo.IsAttributeDefined<SingletonAttribute>(false))
                        {
                            e1.Object = nos.GetSingleton(e1.ObjectType);
                        }
                    }

                    void Nos_Disposed(object sender, EventArgs e)
                    {
                        nos.Disposed -= Nos_Disposed;
                        nos.ObjectByKeyGetting -= Nos_ObjectByKeyGetting;
                    }
                }
            }

            void Application_Disposed(object sender, EventArgs e)
            {
                application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
                application.Disposed -= Application_Disposed;
            }

            base.Setup(application);
        }


    }
}
