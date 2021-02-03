using System;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Shouldly;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.ColumnItems
{
    [DomainComponent]
    [ListViewColumnsBuilder(typeof(SimpleBusinessObjectColumnsBuilder))]
    public sealed class SimpleBusinessObject
    {
        public string? StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public bool? NullableBoolProperty { get; set; }
        public int IntegerProperty { get; set; }
        public int? NullableIntegerProperty { get; set; }
        public object? ObjectProperty { get; set; }
    }

    public static class SimpleBusinessObjectColumnsBuilder
    {
        internal static bool BuildColumnsWasCalled;
        public static Columns BuildColumns()
        {
            BuildColumnsWasCalled = true;
            //var column = new Column(nameof(SimpleBusinessObject.StringProperty));

            return new()
            {
                //column
            };
        }
    }

    public static class BasicColumnsFacts
    {
        internal static IModelListView? FindListView(this IModelApplication model, Type boType)
            => model
                .Views
                .OfType<IModelListView>()
                .FirstOrDefault(d => d.Id.Equals(ModelNodeIdHelper.GetListViewId(boType), StringComparison.Ordinal));

        internal static IModelListView? FindListView<TModelType>(this IModelApplication model)
            where TModelType : class
                => model.FindListView(typeof(TModelType));

        public static void BasicColumsTests() => Describe("Basic Layouts", () =>
        {
            It($"creates {nameof(IModelApplication)}", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObject) });

                model.ShouldBeAssignableTo<IModelApplication>();
            });

            Describe("use generator buddy type logic", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObject) });

                It($"Finds {typeof(SimpleBusinessObject)}  ListView", () =>
                {
                    var detailView = model.FindListView<SimpleBusinessObject>();

                    detailView.ShouldNotBeNull();
                });

                It("static buddy builder was called", () =>
                {
                    var listView = model.FindListView<SimpleBusinessObject>();

                    var _ = listView?.Columns?.FirstOrDefault(); //We need to access the columns node cause it's lazy evaluated

                    SimpleBusinessObjectColumnsBuilder.BuildColumnsWasCalled.ShouldBeTrue();
                });
            });
        });
    }
}
