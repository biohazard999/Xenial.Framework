using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

using Xenial.Framework;

namespace Acme.Module.Helpers;

using DXSystemModele = DevExpress.ExpressApp.SystemModule.SystemModule;

#nullable enable

internal static class ApplicationModelCreator
{
    public static IModelApplication CreateModel(params Type[] boModelTypes)
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
            new TestModule(boModelTypes)
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

        return (IModelApplication)modelManager.CreateModelApplication(
            Enumerable.Empty<ModelApplicationBase>()
        );
    }

    internal sealed class TestModule : XenialModuleBase
    {
        private readonly IEnumerable<Type> boModelTypes;

        internal TestModule(IEnumerable<Type> boModelTypes)
            => this.boModelTypes = boModelTypes;

        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
                .Concat(boModelTypes);

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.UseDetailViewLayoutBuilders();
        }
    }
}
