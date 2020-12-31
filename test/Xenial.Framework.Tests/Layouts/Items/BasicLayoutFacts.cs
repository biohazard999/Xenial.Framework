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

using static Xenial.Framework.Tests.Layouts.Items.TestModelApplicationFactory;
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

    public static class SimpleBusinessObjectLayoutBuilder
    {
        public static Layout BuildLayout()
        {
            var item = LayoutPropertyEditorItem<SimpleBusinessObject>
                .CreatePropertyEditor(p => p.BoolProperty) with
            {
            };

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
