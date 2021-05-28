using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Shouldly;

using Xenial.Framework.ModelBuilders;

using static Xenial.Tasty;

#pragma warning disable CA2000

namespace Xenial.Framework.Tests.ModelBuilders
{
    /// <summary>   A property builder extensions facts. </summary>
    public static class PropertyBuilderExtensionsFacts
    {
        private static (IModelBuilder<ModelBuilderTarget>, Faker faker) CreateBuilder()
                 => (ModelBuilder.Create<ModelBuilderTarget>(new TypesInfo()), new Faker());

        /// <summary>   Property builder extensions tests. </summary>
        public static void PropertyBuilderExtensionsTests() => Describe(nameof(PropertyBuilderExtensions), () =>
        {
            Describe(nameof(ModelDefaultAttribute), () =>
            {
                Describe("string properties", () =>
                {
                    It("non nullable", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var attributeName = faker.Random.String();
                        var attributeValue = faker.Random.String();

                        builder.For(p => p.StringProperty)
                            .WithModelDefault(attributeName, attributeValue)
                            .AssertModelDefaultAttribute(attributeName, attributeValue);
                    });

                    It("nullable", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var attributeName = faker.Random.String();
                        var attributeValue = faker.Random.String();

                        builder.For(p => p.NullableStringProperty)
                            .WithModelDefault(attributeName, attributeValue)
                            .AssertModelDefaultAttribute(attributeName, attributeValue);
                    });
                });

                Describe("bool properties", () =>
                {
                    It("non nullable", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var attributeName = faker.Random.String();
                        var attributeValue = faker.Random.Bool();

                        builder.For(p => p.BoolProperty)
                            .WithModelDefault(attributeName, attributeValue)
                            .AssertModelDefaultAttribute(attributeName, attributeValue.ToString());
                    });

                    It("nullable", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var attributeName = faker.Random.String();
                        var attributeValue = faker.Random.Bool();

                        builder.For(p => p.NullableBoolProperty)
                            .WithModelDefault(attributeName, attributeValue)
                            .AssertModelDefaultAttribute(attributeName, attributeValue.ToString());
                    });
                });

                It("Caption", () =>
                {
                    var (builder, faker) = CreateBuilder();

                    var attributeValue = faker.Random.String();

                    builder.For(p => p.StringProperty)
                        .HasCaption(attributeValue)
                        .AssertModelDefaultAttribute("Caption", attributeValue);
                });

                It("IsPassword", () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder.For(p => p.StringProperty)
                        .IsPassword()
                        .AssertModelDefaultAttribute("IsPassword", true.ToString());
                });

                Describe("PredefinedValues", () =>
                {
                    It("with string values", () =>
                    {
                        var (builder, _) = CreateBuilder();

                        var expectedValue = "foo;bar";

                        builder.For(p => p.StringProperty)
                            .WithPredefinedValues(expectedValue)
                            .AssertModelDefaultAttribute("PredefinedValues", expectedValue);
                    });

                    It("with array values", () =>
                    {
                        var (builder, _) = CreateBuilder();

                        var actualValue = new[] { "foo", "bar", "baz" };
                        var expectedValue = "foo;bar;baz";

                        builder.For(p => p.StringProperty)
                            .WithPredefinedValues(expectedValue)
                            .AssertModelDefaultAttribute("PredefinedValues", expectedValue);
                    });
                });

                Describe("ToolTips", () =>
                {
                    It("has ToolTip", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var expectedValue = faker.Random.String();

                        builder.For(p => p.StringProperty)
                            .HasTooltip(expectedValue)
                            .AssertModelDefaultAttribute("ToolTip", expectedValue);
                    });

                    It("has ToolTip title", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var expectedValue = faker.Random.String();

                        builder.For(p => p.StringProperty)
                            .HasTooltipTitle(expectedValue)
                            .AssertModelDefaultAttribute("ToolTipTitle", expectedValue);
                    });

                    It("has ToolTip title", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var expectedValue = faker.Random.Enum<ToolTipIconType>();

                        builder.For(p => p.StringProperty)
                            .HasTooltipIconType(expectedValue)
                            .AssertModelDefaultAttribute("ToolTipIconType", expectedValue.ToString());
                    });
                });

                It("DisplayFormat", () =>
                {
                    var (builder, _) = CreateBuilder();

                    var expectedValue = "{0:MyFormat}";

                    builder.For(p => p.StringProperty)
                        .HasDisplayFormat(expectedValue)
                        .AssertModelDefaultAttribute("DisplayFormat", expectedValue);
                });

                It("PropertyEditor", () =>
                {
                    It("using type overload", () =>
                    {
                        var (builder, _) = CreateBuilder();

                        builder.For(p => p.StringProperty)
                            .UsingPropertyEditor(typeof(FakePropertyEditor))
                            .AssertModelDefaultAttribute("PropertyEditorType", typeof(FakePropertyEditor).FullName!);
                    });

                    It("using string overload", () =>
                    {
                        var (builder, _) = CreateBuilder();

                        builder.For(p => p.StringProperty)
                            .UsingPropertyEditor(typeof(FakePropertyEditor).FullName!)
                            .AssertModelDefaultAttribute("PropertyEditorType", typeof(FakePropertyEditor).FullName!);
                    });

                    It("using generic overload", () =>
                    {
                        var (builder, _) = CreateBuilder();

                        builder.For(p => p.StringProperty)
                            .UsingPropertyEditor(o => o.Editor<FakePropertyEditor>())
                            .AssertModelDefaultAttribute("PropertyEditorType", typeof(FakePropertyEditor).FullName!);
                    });
                });

                It("Index", () =>
                {
                    var (builder, faker) = CreateBuilder();

                    var expectedValue = faker.Random.Int();

                    builder.For(p => p.StringProperty)
                        .HasIndex(expectedValue)
                        .AssertAttribute<IndexAttribute, string>(attr => attr.Index == expectedValue);
                });

                Describe("Visibility", () =>
                {
                    Describe("InDetailView", () =>
                    {
                        It("visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsVisibleInDetailView()
                                .AssertAttribute<VisibleInDetailViewAttribute, string>(a => (bool)a.Value == true);
                        });
                        It("not visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsNotVisibleInDetailView()
                                .AssertAttribute<VisibleInDetailViewAttribute, string>(a => (bool)a.Value == false);
                        });
                    });

                    Describe("InListView", () =>
                    {
                        It("visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsVisibleInListView()
                                .AssertAttribute<VisibleInListViewAttribute, string>(a => (bool)a.Value == true);
                        });
                        It("not visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsNotVisibleInListView()
                                .AssertAttribute<VisibleInListViewAttribute, string>(a => (bool)a.Value == false);
                        });
                    });

                    Describe("InLookupListView", () =>
                    {
                        It("visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsVisibleInLookupListView()
                                .AssertAttribute<VisibleInLookupListViewAttribute, string>(a => (bool)a.Value == true);
                        });
                        It("not visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsNotVisibleInLookupListView()
                                .AssertAttribute<VisibleInLookupListViewAttribute, string>(a => (bool)a.Value == false);
                        });
                    });

                    Describe("In any View", () =>
                    {
                        It("visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsVisibleInAnyView()
                                .AssertAttribute<VisibleInDetailViewAttribute, string>(a => (bool)a.Value == true)
                                .AssertAttribute<VisibleInListViewAttribute, string>(a => (bool)a.Value == true)
                                .AssertAttribute<VisibleInLookupListViewAttribute, string>(a => (bool)a.Value == true)
                                ;
                        });
                        It("not visible", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .IsNotVisibleInAnyView()
                                .AssertAttribute<VisibleInDetailViewAttribute, string>(a => (bool)a.Value == false)
                                .AssertAttribute<VisibleInListViewAttribute, string>(a => (bool)a.Value == false)
                                .AssertAttribute<VisibleInLookupListViewAttribute, string>(a => (bool)a.Value == false)
                                ;
                        });
                    });

                    Describe("AllowEdit", () =>
                    {
                        It("allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .AllowingEdit()
                                .AssertModelDefaultAttribute("AllowEdit", true.ToString());
                        });

                        It("not allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .NotAllowingEdit()
                                .AssertModelDefaultAttribute("AllowEdit", false.ToString());
                        });
                    }); Describe("AllowEdit", () =>
                    {
                        It("allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .AllowingEdit()
                                .AssertModelDefaultAttribute("AllowEdit", true.ToString());
                        });

                        It("not allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .NotAllowingEdit()
                                .AssertModelDefaultAttribute("AllowEdit", false.ToString());
                        });
                    });

                    Describe("AllowNew", () =>
                    {
                        It("allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .AllowingNew()
                                .AssertModelDefaultAttribute("AllowNew", true.ToString());
                        });

                        It("not allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .NotAllowingNew()
                                .AssertModelDefaultAttribute("AllowNew", false.ToString());
                        });
                    });

                    Describe("AllowDelete", () =>
                    {
                        It("allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .AllowingDelete()
                                .AssertModelDefaultAttribute("AllowDelete", true.ToString());
                        });

                        It("not allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .NotAllowingDelete()
                                .AssertModelDefaultAttribute("AllowDelete", false.ToString());
                        });
                    });

                    Describe("AllowEverything", () =>
                    {
                        It("allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .AllowingEverything()
                                .AssertModelDefaultAttribute("AllowEdit", true.ToString())
                                .AssertModelDefaultAttribute("AllowNew", true.ToString())
                                .AssertModelDefaultAttribute("AllowDelete", true.ToString())
                                ;
                        });
                        It("not allowed", () =>
                        {
                            var (builder, _) = CreateBuilder();

                            builder.For(p => p.StringProperty)
                                .AllowingNothing()
                                .AssertModelDefaultAttribute("AllowEdit", false.ToString())
                                .AssertModelDefaultAttribute("AllowNew", false.ToString())
                                .AssertModelDefaultAttribute("AllowDelete", false.ToString())
                                ;
                        });
                    });

                    It("EditorAlias", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var expectedValue = faker.Random.String();

                        builder.For(p => p.StringProperty)
                            .UsingEditorAlias(expectedValue)
                            .AssertAttribute<EditorAliasAttribute, string>(a => a.Alias == expectedValue);
                    });

                    It("ImmediatePostsData", () =>
                    {
                        var (builder, faker) = CreateBuilder();

                        var expectedValue = faker.Random.String();

                        builder.For(p => p.StringProperty)
                            .ImmediatePostsData()
                            .AssertHasAttribute<ImmediatePostDataAttribute>();
                    });
                });
            });
        });
    }
}
