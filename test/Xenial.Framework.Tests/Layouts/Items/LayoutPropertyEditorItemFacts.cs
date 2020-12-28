using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;

using Shouldly;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Tests.Assertions.Xml;

using static Xenial.Tasty;

using DXSystemModele = DevExpress.ExpressApp.SystemModule.SystemModule;

namespace Xenial.Framework.Tests.Layouts.Items
{
    [DomainComponent]
    public sealed class LayoutPropertyEditorItemBusinessObject
    {
        public string? StringProperty { get; set; }
    }

    public static class LayoutPropertyEditorItemFacts
    {
        internal sealed class TestModule : XenialModuleBase
        {
            private readonly IEnumerable<Type> boModelTypes;
            private readonly Action<ITypesInfo>? customizeTypesInfo;

            public TestModule(IEnumerable<Type> boModelTypes, Action<ITypesInfo>? customizeTypesInfo)
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

        public static void LayoutPropertyEditorItemTests() => FDescribe(nameof(LayoutPropertyEditorItem), () =>
        {
            static IModelApplication CreateApplication(Type[] boModelTypes, Action<ITypesInfo>? customizeTypesInfo = null)
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

            It("gets created with ModelBuilder", () =>
            {
                var model = CreateApplication(new[]
                {
                    typeof(LayoutPropertyEditorItemBusinessObject)
                },
                typesInfo =>
                {
                    ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                        .WithDetailViewLayout(() => new Layout
                        {
                            new LayoutPropertyEditorItem(nameof(LayoutPropertyEditorItemBusinessObject.StringProperty))
                        })
                    .Build();
                });

                var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();

                var xml = UserDifferencesHelper.GetUserDifferences(detailView)[""];
                var prettyXml = new XmlFormatter().Format(xml);
            });
        });
    }
}
