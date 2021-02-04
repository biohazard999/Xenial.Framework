using DevExpress.ExpressApp.Model;

using System;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Tests.Layouts.ColumnItems;

namespace Xenial.Framework.Tests.Layouts
{
    internal static partial class TestModelApplicationFactory
    {
        internal static IModelListView? CreateListViewWithColumns(Func<ColumnsBuilder<LayoutPropertyEditorItemBusinessObject>, Columns> columnsFunctor)
        {
            var model = CreateApplication(new(new[]
            {
                typeof(LayoutPropertyEditorItemBusinessObject)
            },
            typesInfo =>
            {
                ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                    .RemoveAttribute(typeof(ListViewColumnsBuilderAttribute))
                    .WithListViewColumns(columnsFunctor)
                .Build();
            }));

            var listView = model.FindListView<LayoutPropertyEditorItemBusinessObject>();
            return listView;
        }


        //internal static void AssertLayoutItemProperties<TModelType, TTargetModelType>(this IModelDetailView? modelDetailView, Func<ExpressionHelper<TTargetModelType>, Dictionary<string, object>> asserter)
        //    where TModelType : IModelViewLayoutElement
        //{
        //    modelDetailView.ShouldSatisfyAllConditions(
        //        () => modelDetailView.ShouldNotBeNull(),
        //        () => modelDetailView!.Layout.ShouldNotBeNull(),
        //        () => modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName].ShouldNotBeNull(),
        //        () => modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName].ShouldBeAssignableTo<IModelLayoutGroup>()
        //    );

        //    var mainLayoutGroupNode = (IModelLayoutGroup)modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName];
        //    var targetNode = mainLayoutGroupNode.GetNodes<TModelType>().FirstOrDefault();

        //    targetNode.ShouldNotBeNull();

        //    var helper = ExpressionHelper.Create<TTargetModelType>();
        //    var assertions = asserter(helper);

        //    var hasValueAssertions = assertions
        //        .Select(a => new Action(
        //            () => targetNode!.HasValue(a.Key)
        //        )).ToArray();

        //    targetNode.ShouldSatisfyAllConditions(hasValueAssertions);

        //    var valueAssertions = assertions
        //        .Select(a => new Action(
        //            () => targetNode!
        //                .GetValue<object>(a.Key)
        //                .ShouldBe(a.Value, $"'{a.Key}' should be '{a.Value}' but was not.")
        //            )
        //        ).ToArray();

        //    targetNode.ShouldSatisfyAllConditions(valueAssertions);
        //}
    }
}
