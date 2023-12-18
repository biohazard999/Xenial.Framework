using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;

using Shouldly;

using Xenial.Data;
using Xenial.Framework.Tests.Assertions.Xml;
using Xenial.Utils;

namespace Xenial.Framework.Tests.Assertions
{
    internal static class ModelNodeAssertions
    {
        internal static IModelDetailView? FindDetailView(this IModelApplication model, Type boType)
            => model
                .Views
                .OfType<IModelDetailView>()
                .FirstOrDefault(d => d.Id.Equals(ModelNodeIdHelper.GetDetailViewId(boType), StringComparison.Ordinal));

        internal static IModelDetailView? FindDetailView<TModelType>(this IModelApplication model)
            where TModelType : class
                => model.FindDetailView(typeof(TModelType));

        internal static IModelListView? FindListView(this IModelApplication model, Type boType)
            => model
                .Views
                .OfType<IModelListView>()
                .FirstOrDefault(d => d.Id.Equals(ModelNodeIdHelper.GetListViewId(boType), StringComparison.Ordinal));

        internal static IModelListView? FindListView<TModelType>(this IModelApplication model)
            where TModelType : class
                => model.FindListView(typeof(TModelType));

        internal static (string html, string xml) VisualizeModelNode(this IModelNode? modelNode)
        {
            _ = modelNode ?? throw new ArgumentNullException(nameof(modelNode));

            //Force generation before generating the differences
            //To make sure all nodes are created
            //This does not happen in production code
            if (modelNode is IModelDetailView modelDetailView)
            {
                _ = modelDetailView.Items?.FirstOrDefault();
                _ = modelDetailView.Layout?.FirstOrDefault();
            }
            if (modelNode is IModelListView modelListView)
            {
                _ = modelListView.Columns?.FirstOrDefault();
            }

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
            return (html, prettyXml);
        }


        internal static void AssertLayoutItemProperties<TModelType, TTargetModelType>(
            this IModelDetailView? modelDetailView,
            Func<ExpressionHelper<TTargetModelType>, Dictionary<string, object>> asserter
        ) where TModelType : IModelViewLayoutElement
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

        internal static void AssertColumnProperties<TModelType, TTargetModelType>(
            this IModelListView? modelListView,
            Func<ExpressionHelper<TTargetModelType>, Dictionary<string, object>> asserter
        ) where TModelType : IModelColumn
        {
            modelListView.ShouldSatisfyAllConditions(
                () => modelListView.ShouldNotBeNull()
            );

            var targetNode = modelListView!.Columns.FirstOrDefault();

            targetNode.ShouldNotBeNull("No columns found");

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
    }
}
