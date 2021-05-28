using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.ModelBuilders;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.ModelBuilders
{
    /// <summary>   A model builder extension facts. </summary>
    public static class ModelBuilderExtensionFacts
    {
        /// <summary>   Model builder extension tests. </summary>
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

            Describe(nameof(VisibleInReportsAttribute), () =>
            {
                It("should be true", () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .IsVisibleInReports()
                        .AssertAttribute<VisibleInReportsAttribute>(a => a.IsVisible == true);
                });

                It("shoule be false", () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .IsVisibleInReports()
                        .AssertAttribute<VisibleInReportsAttribute>(a => a.IsVisible == true);
                });
            });

            Describe(nameof(VisibleInDashboardsAttribute), () =>
            {
                It("should be true", () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .IsVisibleInDashboards()
                        .AssertAttribute<VisibleInDashboardsAttribute>(a => a.IsVisible == true);
                });

                It("shoule be false", () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .IsNotVisibleInDashboards()
                        .AssertAttribute<VisibleInDashboardsAttribute>(a => a.IsVisible == false);
                });
            });

            It(nameof(DefaultClassOptionsAttribute), () =>
            {
                var (builder, _) = CreateBuilder();
                builder
                    .WithDefaultClassOptions()
                    .AssertHasAttribute<DefaultClassOptionsAttribute>();
            });

            It(nameof(ImageNameAttribute), () =>
            {
                var (builder, faker) = CreateBuilder();
                var image = faker.Random.String();

                builder
                    .HasImage(image)
                    .AssertAttribute<ImageNameAttribute>(a => a.ImageName == image);
            });

            Describe("DefaultProperty", () =>
            {
                It("string overload", () =>
                {
                    var (builder, faker) = CreateBuilder();
                    var defaultProperty = faker.Random.String();

                    builder
                        .HasDefaultProperty(defaultProperty)
                        .AssertAttribute<System.ComponentModel.DefaultPropertyAttribute>(a => a.Name == defaultProperty)
                        .AssertAttribute<XafDefaultPropertyAttribute>(a => a.Name == defaultProperty)
                        ;
                });

                It("expression overload", () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .HasDefaultProperty(x => x.ListProperty)
                        .AssertAttribute<System.ComponentModel.DefaultPropertyAttribute>(a => a.Name == nameof(ModelBuilderTarget.ListProperty))
                        .AssertAttribute<XafDefaultPropertyAttribute>(a => a.Name == nameof(ModelBuilderTarget.ListProperty))
                        ;
                });
            });

            Describe(nameof(ObjectCaptionFormatAttribute), () =>
            {
                It("string overload", () =>
                {
                    var (builder, faker) = CreateBuilder();
                    var formatString = faker.Random.String();

                    builder
                        .HasObjectCaptionFormat(formatString)
                        .AssertAttribute<ObjectCaptionFormatAttribute>(a => a.FormatString == formatString)
                        ;
                });

                It("expression overload", () =>
                {
                    var (builder, _) = CreateBuilder();

                    builder
                        .HasObjectCaptionFormat(x => x.ListProperty)
                        .AssertAttribute<ObjectCaptionFormatAttribute>(a => a.FormatString == $"{{0:{nameof(ModelBuilderTarget.ListProperty)}}}")
                        ;
                });
            });
        });
    }
}
