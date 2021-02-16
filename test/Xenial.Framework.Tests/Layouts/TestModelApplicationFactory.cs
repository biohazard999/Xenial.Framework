using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

using DXSystemModele = DevExpress.ExpressApp.SystemModule.SystemModule;

namespace Xenial.Framework.Tests.Layouts
{
    internal record CreateApplicationOptions(Type[] BoModelTypes, Action<ITypesInfo>? CustomizeTypesInfo = null)
    {
        internal Action<ModelNodesGeneratorUpdaters>? CustomizeGeneratorUpdaters { get; set; }
    }

    internal static partial class TestModelApplicationFactory
    {
        internal static IModelApplication CreateApplication(Type[] boModelTypes, Action<ITypesInfo>? customizeTypesInfo = null)
            => CreateApplication(new(boModelTypes, customizeTypesInfo));

        internal static IModelApplication CreateApplication(CreateApplicationOptions options)
        {
            XafTypesInfo.HardReset();

            if (XafTypesInfo.Instance is TypesInfo typesInfo)
            {
                var store = typesInfo.FindEntityStore(typeof(NonPersistentTypeInfoSource));
                if (store is not null)
                {
                    foreach (var type in options.BoModelTypes)
                    {
                        store.RegisterEntity(type);
                    }
                }
            }

            var modelManager = new ApplicationModelManager(null, true);

            var modules = new ModuleBase[]
            {
                new DXSystemModele(),
                new TestModule(options.BoModelTypes, options.CustomizeTypesInfo)
                {
                    CustomizeGeneratorUpdaters = options.CustomizeGeneratorUpdaters
                }
            };

            foreach (var module in modules)
            {
                module.CustomizeTypesInfo(XafTypesInfo.Instance);
            }

            modelManager.Setup(
                XafTypesInfo.Instance,
                options.BoModelTypes,
                modules,
                Enumerable.Empty<Controller>(),
                Enumerable.Empty<Type>(),
                Enumerable.Empty<string>(),
                null,
                null
            );

            return (IModelApplication)modelManager.CreateModelApplication(Enumerable.Empty<ModelApplicationBase>());
        }
    }
}
