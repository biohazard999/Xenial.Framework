using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

using DXSystemModele = DevExpress.ExpressApp.SystemModule.SystemModule;

namespace Xenial.Framework.Tests.Layouts.Items
{
    internal static class TestModelApplicationFactory
    {
        internal static IModelApplication CreateApplication(Type[] boModelTypes, Action<ITypesInfo>? customizeTypesInfo = null)
        {
            XafTypesInfo.HardReset();

            if (XafTypesInfo.Instance is TypesInfo typesInfo)
            {
                var store = typesInfo.FindEntityStore(typeof(NonPersistentTypeInfoSource));
                if (store is not null)
                {
                    foreach (var type in boModelTypes)
                    {
                        store.RegisterEntity(type);
                    }
                }
            }

            var modelManager = new ApplicationModelManager(null, true);

            var modules = new ModuleBase[]
            {
                    new DXSystemModele(),
                    new TestModule(boModelTypes, customizeTypesInfo)
            };

            foreach (var module in modules)
            {
                module.CustomizeTypesInfo(XafTypesInfo.Instance);
            }

            modelManager.Setup(
                XafTypesInfo.Instance,
                boModelTypes,
                modules,
                Enumerable.Empty<Controller>(),
                Enumerable.Empty<Type>(),
                Enumerable.Empty<string>(),
                null,
                null
            );

            return (IModelApplication)modelManager.CreateModelApplication(Enumerable.Empty<ModelApplicationBase>());
        }

        internal sealed class TestModule : XenialModuleBase
        {
            private readonly IEnumerable<Type> boModelTypes;
            private readonly Action<ITypesInfo>? customizeTypesInfo;

            internal TestModule(
                IEnumerable<Type> boModelTypes,
                Action<ITypesInfo>? customizeTypesInfo = null
            )
            {
                this.boModelTypes = boModelTypes;
                this.customizeTypesInfo = customizeTypesInfo;
            }

            protected override IEnumerable<Type> GetDeclaredExportedTypes()
                => base.GetDeclaredExportedTypes()
                    .Concat(boModelTypes);

            protected override IEnumerable<Type> GetRegularTypes()
                => base.GetRegularTypes()
                    .UseDetailViewLayoutBuildersRegularTypes();

            public override void CustomizeTypesInfo(ITypesInfo typesInfo)
            {
                base.CustomizeTypesInfo(typesInfo);

                customizeTypesInfo?.Invoke(typesInfo);
            }

            public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
            {
                base.AddGeneratorUpdaters(updaters);
                updaters.UseDetailViewLayoutBuilders();
            }

            public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
            {
                base.ExtendModelInterfaces(extenders);
                extenders.UseDetailViewLayoutBuilders();
            }
        }
    }
}
