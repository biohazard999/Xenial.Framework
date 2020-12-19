using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.ModelBuilders
{
    public static class ModelBuilderExtensionFacts
    {
        public static void ModelBuilderExtensionTests() => Describe(nameof(ModelBuilderExtensions), () =>
        {
            static (IModelBuilder<ModelBuilderTarget>, Faker faker) CreateBuilder()
                => (ModelBuilder.Create<ModelBuilderTarget>(new TypesInfo()), new Faker());

            Describe(nameof(ModelBuilderExtensions.WithModelDefault), () =>
            {
                It("adds string value", () =>
                {
                    var (builder, faker) = CreateBuilder();

                    var attributeName = faker.Random.String();
                    var attributeValue = faker.Random.String();

                    builder
                        .WithModelDefault(attributeName, attributeValue)
                        .AssertModelDefaultAttribute(attributeName, attributeValue);
                });

                It("adds boolean value", () =>
                {
                    var (builder, faker) = CreateBuilder();

                    var attributeName = faker.Random.String();
                    var attributeValue = faker.Random.Bool();

                    builder
                        .WithModelDefault(attributeName, attributeValue)
                        .AssertModelDefaultAttribute(attributeName, attributeValue.ToString());
                });
            });

            It(nameof(ModelBuilderExtensions.HasCaption), () =>
            {
                var (builder, faker) = CreateBuilder();

                var attributeValue = faker.Random.String();

                builder
                    .HasCaption(attributeValue)
                    .AssertModelDefaultAttribute("Caption", attributeValue.ToString());
            });

            Describe("Edit", () =>
            {
                It(nameof(ModelBuilderExtensions.AllowingEdit), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .AllowingEdit()
                        .AssertModelDefaultAttribute("AllowEdit", true.ToString());
                });

                It(nameof(ModelBuilderExtensions.NotAllowingEdit), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .NotAllowingEdit()
                        .AssertModelDefaultAttribute("AllowEdit", false.ToString());
                });
            });

            Describe("New", () =>
            {
                It(nameof(ModelBuilderExtensions.AllowingNew), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .AllowingNew()
                        .AssertModelDefaultAttribute("AllowNew", true.ToString());
                });

                It(nameof(ModelBuilderExtensions.NotAllowingNew), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .NotAllowingNew()
                        .AssertModelDefaultAttribute("AllowNew", false.ToString());
                });
            });

            Describe("Delete", () =>
            {
                It(nameof(ModelBuilderExtensions.AllowingDelete), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .AllowingDelete()
                        .AssertModelDefaultAttribute("AllowDelete", true.ToString());
                });

                It(nameof(ModelBuilderExtensions.NotAllowingDelete), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .NotAllowingDelete()
                        .AssertModelDefaultAttribute("AllowDelete", false.ToString());
                });
            });

            Describe("Allow everything/nothing", () =>
            {
                It(nameof(ModelBuilderExtensions.AllowingEverything), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .AllowingEverything()
                        .AssertModelDefaultAttribute("AllowDelete", true.ToString())
                        .AssertModelDefaultAttribute("AllowNew", true.ToString())
                        .AssertModelDefaultAttribute("AllowEdit", true.ToString())
                        ;
                });

                It(nameof(ModelBuilderExtensions.AllowingNothing), () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .AllowingNothing()
                        .AssertModelDefaultAttribute("AllowDelete", false.ToString())
                        .AssertModelDefaultAttribute("AllowNew", false.ToString())
                        .AssertModelDefaultAttribute("AllowEdit", false.ToString())
                        ;
                });
            });
        });
    }
}
