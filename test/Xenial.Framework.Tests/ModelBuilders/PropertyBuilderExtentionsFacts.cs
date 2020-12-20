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

namespace Xenial.Framework.Tests.ModelBuilders
{
    public static class PropertyBuilderExtensionsFacts
    {
        static (IModelBuilder<ModelBuilderTarget>, Faker faker) CreateBuilder()
                 => (ModelBuilder.Create<ModelBuilderTarget>(new TypesInfo()), new Faker());

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
            });
        });
    }

    public class FakePropertyEditor : PropertyEditor
    {
        public FakePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override object CreateControlCore() => throw new NotImplementedException();
        protected override object GetControlValueCore() => throw new NotImplementedException();
        protected override void ReadValueCore() => throw new NotImplementedException();
    }
}
