using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

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
            });
        });
    }
}
