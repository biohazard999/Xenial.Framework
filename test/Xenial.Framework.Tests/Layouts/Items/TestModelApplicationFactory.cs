using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;

using Shouldly;

using Xenial.Data;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Tests.Assertions.Xml;
using Xenial.Utils;

using DXSystemModele = DevExpress.ExpressApp.SystemModule.SystemModule;

namespace Xenial.Framework.Tests.Layouts.Items
{
    [DomainComponent]
    public sealed class LayoutPropertyEditorItemBusinessObject
    {
        public string? StringProperty { get; set; }
    }

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

        internal static IModelDetailView? CreateDetailViewWithLayout(Func<LayoutBuilder<LayoutPropertyEditorItemBusinessObject>, Layout> layoutFunctor)
        {
            var model = CreateApplication(new[]
            {
                typeof(LayoutPropertyEditorItemBusinessObject)
            },
            typesInfo =>
            {
                ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                    .WithDetailViewLayout(layoutFunctor)
                .Build();
            });

            var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();
            return detailView;
        }

        internal static IModelDetailView? CreateComplexDetailViewWithLayout(Func<LayoutBuilder<SimpleBusinessObject>, Layout> layoutFunctor)
        {
            var model = CreateApplication(new[]
            {
                typeof(SimpleBusinessObject)
            },
            typesInfo =>
            {
                ModelBuilder.Create<SimpleBusinessObject>(typesInfo)
                    .WithDetailViewLayout(layoutFunctor)
                .Build();
            });

            var detailView = model.FindDetailView<SimpleBusinessObject>();
            return detailView;
        }

        internal static void VisualizeModelNode(this IModelNode? modelNode)
        {
            _ = modelNode ?? throw new ArgumentNullException(nameof(modelNode));
            var xml = UserDifferencesHelper.GetUserDifferences(modelNode)[""];
            var prettyXml = new XmlFormatter().Format(xml);
            var encode = WebUtility.HtmlEncode(prettyXml);
            var html = @$"
<html>
    <head>
        <link href='https://unpkg.com/prismjs@1.22.0/themes/prism-okaidia.css' rel='stylesheet' />
    </head>
    <body style='background-color: #272822; color: #bbb; font-family: sans-serif; margin: 0; padding: 0;'>
        <h1 style='text-align: center; margin-top: .5rem'>XAF Layout Inspector</h1>
        <hr style='border: none; border-top: 1px solid #bbb;' />
        <pre><code class='language-xml'>{encode}</code></pre>
        <script src='https://unpkg.com/prismjs@1.22.0/components/prism-core.min.js'></script>
        <script src='https://unpkg.com/prismjs@1.22.0/plugins/autoloader/prism-autoloader.min.js'></script>
    </body>
</html>";
#if DEBUG
            System.IO.File.WriteAllText(@"C:\F\tmp\Xenial\1.html", html);
#endif
        }

        internal static void AssertLayoutItemProperties<TModelType, TTargetModelType>(this IModelDetailView? modelDetailView, Func<ExpressionHelper<TTargetModelType>, Dictionary<string, object>> asserter)
            where TModelType : IModelViewLayoutElement
        {
            modelDetailView.ShouldSatisfyAllConditions(
                () => modelDetailView.ShouldNotBeNull(),
                () => modelDetailView!.Layout.ShouldNotBeNull(),
                () => modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName].ShouldNotBeNull(),
                () => modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName].ShouldBeAssignableTo<IModelLayoutGroup>()
            );

            var mainLayoutGroupNode = (IModelLayoutGroup)modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName];
            var targetNode = mainLayoutGroupNode.GetNodes<TModelType>().FirstOrDefault();

            targetNode.ShouldNotBeNull();

            var helper = ExpressionHelper.Create<TTargetModelType>();
            var assertions = asserter(helper);

            var hasValueAssertions = assertions
                .Select(a => new Action(
                    () => targetNode!.HasValue(a.Key)
                )).ToArray();

            targetNode.ShouldSatisfyAllConditions(hasValueAssertions);

            var valueAssertions = assertions
                .Select(a => new Action(
                    () => targetNode!
                        .GetValue<object>(a.Key)
                        .ShouldBe(a.Value, $"'{a.Key}' should be '{a.Value}' but was not.")
                    )
                ).ToArray();

            targetNode.ShouldSatisfyAllConditions(valueAssertions);
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
