using System;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Shouldly;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.Model;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    [DomainComponent]
    [DetailViewLayoutBuilder(typeof(SimpleBusinessObjectLayoutBuilder))]
    public sealed class SimpleBusinessObject
    {
        public string? StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public bool? NullableBoolProperty { get; set; }
        public int IntegerProperty { get; set; }
        public int? NullableIntegerProperty { get; set; }
        public object? ObjectProperty { get; set; }
    }

    [DomainComponent]
    [DetailViewLayoutBuilder(nameof(BuildExoticLayout))]
    public sealed class SimpleBusinessObjectWithStaticBuilder
    {
        public string? StringProperty { get; set; }

        internal static bool BuildExoticLayoutWasCalled;

        public static Layout BuildExoticLayout()
        {
            BuildExoticLayoutWasCalled = true;
            return new();
        }
    }

    [DomainComponent]
    [DetailViewLayoutBuilder]
    public sealed class SimpleBusinessObjectWithStaticBuilderConvention
    {
        internal static bool BuildLayoutWasCalled;

        public static Layout BuildLayout()
        {
            BuildLayoutWasCalled = true;
            return new();
        }
    }

    public static class SimpleBusinessObjectLayoutBuilder
    {
        internal static bool BuildLayoutWasCalled;
        public static Layout BuildLayout()
        {
            BuildLayoutWasCalled = true;
            var layoutPropertyEditor = new LayoutPropertyEditorItem(nameof(SimpleBusinessObject.StringProperty));

            return new()
            {
                layoutPropertyEditor
            };
        }
    }

    public static class BasicLayoutFacts
    {
        internal static IModelDetailView? FindDetailView(this IModelApplication model, Type boType)
            => model
                .Views
                .OfType<IModelDetailView>()
                .FirstOrDefault(d => d.Id.Equals(ModelNodeIdHelper.GetDetailViewId(boType), StringComparison.Ordinal));

        internal static IModelDetailView? FindDetailView<TModelType>(this IModelApplication model)
            where TModelType : class
                => model.FindDetailView(typeof(TModelType));

        public static void BasicLayoutTests() => Describe("Basic Layouts", () =>
        {
            It($"creates {nameof(IModelApplication)}", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObject) });

                model.ShouldBeAssignableTo<IModelApplication>();
            });

            Describe("use generator buddy type logic", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObject) });

                It($"Finds {typeof(SimpleBusinessObject)} DetailView", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObject>();

                    detailView.ShouldNotBeNull();
                });

                It("static buddy builder was called", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObject>();

                    var _ = detailView?.Layout?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                    SimpleBusinessObjectLayoutBuilder.BuildLayoutWasCalled.ShouldBeTrue();
                });
            });

            Describe("use static type on model class", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObjectWithStaticBuilder) });

                It("returns the detail view", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObjectWithStaticBuilder>();

                    detailView.ShouldNotBeNull();
                });

                It("static builder was called", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObjectWithStaticBuilder>();

                    var _ = detailView?.Layout?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                    SimpleBusinessObjectWithStaticBuilder.BuildExoticLayoutWasCalled.ShouldBeTrue();
                });
            });

            Describe("use static type on model class with convention", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObjectWithStaticBuilderConvention) });

                It("returns the detail view", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObjectWithStaticBuilderConvention>();

                    detailView.ShouldNotBeNull();
                });

                It("static builder was called", () =>
                {
                    var detailView = model.FindDetailView<SimpleBusinessObjectWithStaticBuilderConvention>();

                    var _ = detailView?.Layout?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                    SimpleBusinessObjectWithStaticBuilderConvention.BuildLayoutWasCalled.ShouldBeTrue();
                });
            });
        });
    }
}
