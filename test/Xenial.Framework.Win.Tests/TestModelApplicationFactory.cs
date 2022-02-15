using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Tests.Assertions;
using Xenial.Framework.Tests.Layouts;

using DXSystemModule = DevExpress.ExpressApp.SystemModule.SystemModule;
using DXSystemWindowsFormsModule = DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule;

namespace Xenial.Framework.Win.Tests.Layouts
{
    internal record CreateApplicationOptionsWin(Type[] BoModelTypes, Action<ITypesInfo>? CustomizeTypesInfo = null)
    {
        internal Action<ModelNodesGeneratorUpdaters>? CustomizeGeneratorUpdaters { get; set; }
    }

    internal static partial class TestModelApplicationFactoryWin
    {
        internal static IModelApplication CreateApplication(Type[] boModelTypes, Action<ITypesInfo>? customizeTypesInfo = null)
            => CreateApplication(new(boModelTypes, customizeTypesInfo));

        internal static IModelApplication CreateApplication(CreateApplicationOptionsWin options)
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
                new DXSystemModule(),
                new DXSystemWindowsFormsModule(),
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

        internal static IModelDetailView? CreateComplexDetailViewWithLayout<T>(Func<LayoutBuilder<T>, Layout> layoutFunctor)
            where T : class
        {
            var model = CreateApplication(new(new[]
            {
                typeof(T)
            },
            typesInfo =>
            {
                ModelBuilder.Create<T>(typesInfo)
                    .RemoveAttribute(typeof(DetailViewLayoutBuilderAttribute))
                    .WithDetailViewLayout(layoutFunctor)
                .Build();
            }));

            var detailView = model.FindDetailView<T>();
            return detailView;
        }
    }
}
