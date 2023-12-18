using System;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Shouldly;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.Tests.Assertions;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    [DomainComponent]
    [DetailViewLayoutBuilder(typeof(ComplexBusinessObjectLayoutBuilder))]
    public sealed class ComplexBusinessObject
    {
        public SimpleBusinessObject NestedObject { get; set; } = new();
        public string OwnString { get; set; } = "";
    }

    /// <summary>   A simple business object. This class cannot be inherited. </summary>
    [DomainComponent]
    [DetailViewLayoutBuilder(typeof(SimpleBusinessObjectLayoutBuilder))]
    public sealed class SimpleBusinessObject
    {
        /// <summary>   Gets or sets the string property. </summary>
        ///
        /// <value> The string property. </value>

        public string? StringProperty { get; set; }

        /// <summary>   Gets or sets a value indicating whether the property. </summary>
        ///
        /// <value> True if property, false if not. </value>

        public bool BoolProperty { get; set; }

        /// <summary>   Gets or sets the nullable bool property. </summary>
        ///
        /// <value> The nullable bool property. </value>

        public bool? NullableBoolProperty { get; set; }

        /// <summary>   Gets or sets the integer property. </summary>
        ///
        /// <value> The integer property. </value>

        public int IntegerProperty { get; set; }

        /// <summary>   Gets or sets the nullable integer property. </summary>
        ///
        /// <value> The nullable integer property. </value>

        public int? NullableIntegerProperty { get; set; }

        /// <summary>   Gets or sets the object property. </summary>
        ///
        /// <value> The object property. </value>

        public object? ObjectProperty { get; set; }
    }

    /// <summary>
    /// A simple business object with static builder. This class cannot be inherited.
    /// </summary>

    [DomainComponent]
    [DetailViewLayoutBuilder(nameof(BuildExoticLayout))]
    public sealed class SimpleBusinessObjectWithStaticBuilder
    {
        /// <summary>   Gets or sets the string property. </summary>
        ///
        /// <value> The string property. </value>

        public string? StringProperty { get; set; }

        internal static bool BuildExoticLayoutWasCalled;

        /// <summary>   Builds exotic layout. </summary>
        ///
        /// <returns>   A Layout. </returns>

        public static Layout BuildExoticLayout()
        {
            BuildExoticLayoutWasCalled = true;
            return new();
        }
    }

    /// <summary>
    /// A simple business object with static builder convention. This class cannot be inherited.
    /// </summary>

    [DomainComponent]
    [DetailViewLayoutBuilder]
    public sealed class SimpleBusinessObjectWithStaticBuilderConvention
    {
        internal static bool BuildLayoutWasCalled;

        /// <summary>   Builds the layout. </summary>
        ///
        /// <returns>   A Layout. </returns>

        public static Layout BuildLayout()
        {
            BuildLayoutWasCalled = true;
            return new();
        }
    }

    /// <summary>   A simple business object layout builder. </summary>
    public static class SimpleBusinessObjectLayoutBuilder
    {
        internal static bool BuildLayoutWasCalled;

        /// <summary>   Builds the layout. </summary>
        ///
        /// <returns>   A Layout. </returns>

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

    public static class ComplexBusinessObjectLayoutBuilder
    {
        public static Layout BuildLayout()
        {
            var l = new LayoutBuilder<ComplexBusinessObject>();

            return new()
            {
                l.PropertyEditor(m => m.OwnString),
                l.PropertyEditor(m => m.NestedObject),
                l.PropertyEditor(m => m.NestedObject.StringProperty)
            };
        }
    }

    /// <summary>   A basic layout facts. </summary>
    public static class BasicLayoutFacts
    {
        /// <summary>   Basic layout tests. </summary>
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
