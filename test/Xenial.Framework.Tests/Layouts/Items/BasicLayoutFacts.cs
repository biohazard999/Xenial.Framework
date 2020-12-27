using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Shouldly;

using Xenial.Framework.Layouts;
using Xenial.Framework.Model;

using static Xenial.Tasty;

using DXSystemModele = DevExpress.ExpressApp.SystemModule.SystemModule;

namespace Xenial.Framework.Tests.Layouts.Items
{
    [DomainComponent]
    [DetailViewLayoutBuilder(typeof(SimpleBusinessObjectLayoutBuilder))]
    public sealed class SimpleBusinessObject
    {
        public string? StringProperty { get; set; }
    }

    public sealed class SimpleBusinessObjectLayoutBuilder
    {

    }

    public static class BasicLayoutFacts
    {
        internal sealed class TestModule : XenialModuleBase
        {
            private readonly IEnumerable<Type> boModelTypes;
            public TestModule(IEnumerable<Type> boModelTypes)
                => this.boModelTypes = boModelTypes;
            protected override IEnumerable<Type> GetDeclaredExportedTypes()
                => base.GetDeclaredExportedTypes()
                    .Concat(boModelTypes);

            protected override IEnumerable<Type> GetRegularTypes()
                => base.GetRegularTypes()
                    .UseDetailViewLayoutBuildersRegularTypes();

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

        internal static IModelDetailView? FindDetailView(this IModelApplication model, Type boType)
            => model
                .Views
                .OfType<IModelDetailView>()
                .FirstOrDefault(d => d.Id.Equals(ModelNodeIdHelper.GetDetailViewId(boType), StringComparison.Ordinal));

        internal static IModelDetailView? FindDetailView<TModelType>(this IModelApplication model)
            where TModelType : class
                => model.FindDetailView(typeof(TModelType));

        public static void BasicLayoutTests() => FDescribe("Basic Layouts", () =>
        {
            static IModelApplication CreateApplication(params Type[] boModelTypes)
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

                return (IModelApplication)modelManager.CreateModelApplication(Enumerable.Empty<ModelApplicationBase>());
            }

            It($"creates {nameof(IModelApplication)}", () =>
            {
                var model = CreateApplication(typeof(SimpleBusinessObject));

                model.ShouldBeAssignableTo<IModelApplication>();
            });

            Describe("use generator buddy type logic", () =>
            {
                var model = CreateApplication(typeof(SimpleBusinessObject));

                It($"Finds {typeof(SimpleBusinessObject)} DetailView", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObject>();

                    detailView.ShouldNotBeNull();
                });

                It("Sets the generator flag when attribute is set in code", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObject>();

                    if (detailView is IModelObjectGeneratedView modelGeneratedView)
                    {
                        modelGeneratedView.GeneratorType.ShouldBe(typeof(SimpleBusinessObjectLayoutBuilder));
                    }
                    else
                    {
                        throw new InvalidOperationException("Model extention was not registered correctly");
                    }
                });
            });
        });
    }
}
