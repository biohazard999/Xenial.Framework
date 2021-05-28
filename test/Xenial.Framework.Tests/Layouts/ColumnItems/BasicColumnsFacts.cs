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
using Xenial.Framework.Tests.Assertions;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.ColumnItems
{
    /// <summary>   A simple business object. This class cannot be inherited. </summary>
    [DomainComponent]
    [ListViewColumnsBuilder(typeof(SimpleBusinessObjectColumnsBuilder))]
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

    /// <summary>   A simple business object columns builder. </summary>
    public static class SimpleBusinessObjectColumnsBuilder
    {
        internal static bool BuildColumnsWasCalled;

        /// <summary>   Builds the columns. </summary>
        ///
        /// <returns>   The Columns. </returns>

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

    /// <summary>
    /// A simple business object with static builder. This class cannot be inherited.
    /// </summary>

    [DomainComponent]
    [ListViewColumnsBuilder(nameof(BuildExoticColumns))]
    public sealed class SimpleBusinessObjectWithStaticBuilder
    {
        /// <summary>   Gets or sets the string property. </summary>
        ///
        /// <value> The string property. </value>

        public string? StringProperty { get; set; }

        internal static bool BuildExoticColumnsWasCalled;

        /// <summary>   Builds exotic columns. </summary>
        ///
        /// <returns>   The Columns. </returns>

        public static Columns BuildExoticColumns()
        {
            BuildExoticColumnsWasCalled = true;
            return new();
        }
    }

    /// <summary>
    /// A simple business object with static builder convention. This class cannot be inherited.
    /// </summary>

    [DomainComponent]
    [ListViewColumnsBuilder]
    public sealed class SimpleBusinessObjectWithStaticBuilderConvention
    {
        internal static bool BuildColumnsWasCalled;

        /// <summary>   Builds the columns. </summary>
        ///
        /// <returns>   The Columns. </returns>

        public static Columns BuildColumns()
        {
            BuildColumnsWasCalled = true;
            return new();
        }
    }

    /// <summary>   A basic columns facts. </summary>
    public static class BasicColumnsFacts
    {
        /// <summary>   Basic columns tests. </summary>
        public static void BasicColumnsTests() => Describe("Basic Columns", () =>
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
                    var listView = model.FindListView<SimpleBusinessObject>();

                    listView.ShouldNotBeNull();
                });

                It("static buddy builder was called", () =>
                {
                    var listView = model.FindListView<SimpleBusinessObject>();

                    var _ = listView?.Columns?.FirstOrDefault(); //We need to access the columns node cause it's lazy evaluated

                    SimpleBusinessObjectColumnsBuilder.BuildColumnsWasCalled.ShouldBeTrue();
                });
            });

            Describe("use static type on model class", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObjectWithStaticBuilder) });

                It("returns the list view", () =>
                {
                    var listView = model.FindListView<SimpleBusinessObjectWithStaticBuilder>();

                    listView.ShouldNotBeNull();
                });

                It("static builder was called", () =>
                {
                    var listView = model.FindListView<SimpleBusinessObjectWithStaticBuilder>();

                    var _ = listView?.Columns?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                    SimpleBusinessObjectWithStaticBuilder.BuildExoticColumnsWasCalled.ShouldBeTrue();
                });
            });

            Describe("use static type on model class with convention", () =>
            {
                var model = CreateApplication(new[] { typeof(SimpleBusinessObjectWithStaticBuilderConvention) });

                It("returns the list view", () =>
                {
                    var listView = model.FindListView<SimpleBusinessObjectWithStaticBuilderConvention>();

                    listView.ShouldNotBeNull();
                });

                It("static builder was called", () =>
                {
                    var listView = model.FindListView<SimpleBusinessObjectWithStaticBuilderConvention>();

                    var _ = listView?.Columns?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                    SimpleBusinessObjectWithStaticBuilderConvention.BuildColumnsWasCalled.ShouldBeTrue();
                });
            });
        });
    }
}
