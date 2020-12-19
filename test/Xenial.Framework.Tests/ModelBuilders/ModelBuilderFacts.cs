using System;
using System.Collections;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.Pdf;

using FakeItEasy;

using Shouldly;

using Xenial.Framework.ModelBuilders;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.ModelBuilders
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class EmptyCtorLessAttribute : Attribute
    {
        public string AttributeProperty { get; set; } = string.Empty;
    }

    public class ModelBuilderTarget
    {
        public IList ListProperty { get; } = Array.Empty<object>();
    }

    public static class ModelBuilderFacts
    {
        public static void ModelBuilderTests() => Describe(nameof(ModelBuilder), () =>
        {
            static (ModelBuilder<ModelBuilderTarget>, ITypeInfo) CreateBuilder()
            {
                var typesInfo = A.Fake<ITypesInfo>();
                var typeInfo = A.Fake<ITypeInfo>();

                A.CallTo(() => typesInfo.FindTypeInfo(typeof(ModelBuilderTarget)))
                    .Returns(typeInfo);

                var builder = ModelBuilder.Create<ModelBuilderTarget>(typesInfo);
                return (builder, typeInfo);
            }

            static (ModelBuilder<ModelBuilderTarget>, ITypeInfo) CreateBuilderWithTypesInfo()
            {
                var typesInfo = new TypesInfo();
                var typeInfo = typesInfo.FindTypeInfo(typeof(ModelBuilderTarget));
                var builder = ModelBuilder.Create<ModelBuilderTarget>(typesInfo);
                return (builder, typeInfo);
            }

            Describe(nameof(ModelBuilder.Create), () =>
            {
                It("creates correct instance of builder", () =>
                {
                    var builder = ModelBuilder.Create<ModelBuilderTarget>(A.Fake<ITypesInfo>());
                    builder.ShouldBeOfType(typeof(ModelBuilder<ModelBuilderTarget>));
                });

                It($"{nameof(TypesInfo)} should be correct", () =>
                {
                    var (builder, typeInfo) = CreateBuilder();
                    builder.TypeInfo.ShouldBe(typeInfo);
                });

                It($"TargetType is correct", () =>
                {
                    var (builder, _) = CreateBuilder();
                    builder.TargetType.ShouldBe(typeof(ModelBuilderTarget));
                });
            });

            Describe("ViewIds", () =>
            {
                It("DefaultDetailView", () =>
                {
                    var (builder, _) = CreateBuilder();
                    builder.DefaultDetailView.ShouldBe($"{nameof(ModelBuilderTarget)}_DetailView");
                });

                It("DefaultListView", () =>
                {
                    var (builder, _) = CreateBuilder();
                    builder.DefaultListView.ShouldBe($"{nameof(ModelBuilderTarget)}_ListView");
                });

                It("DefaultLookupListView", () =>
                {
                    var (builder, _) = CreateBuilder();
                    builder.DefaultLookupListView.ShouldBe($"{nameof(ModelBuilderTarget)}_LookupListView");
                });

                It("GetNestedListViewId", () =>
                {
                    var (builder, _) = CreateBuilder();
                    builder.GetNestedListViewId(t => t.ListProperty)
                        .ShouldBe($"{nameof(ModelBuilderTarget)}_{nameof(ModelBuilderTarget.ListProperty)}_ListView");
                });
            });

            Describe("Attributes", () =>
            {
                It("can be added with default ctor", () =>
                {
                    var (builder, typeInfo) = CreateBuilder();

                    builder.WithAttribute<EmptyCtorLessAttribute>();

                    A.CallTo(() => typeInfo.AddAttribute(
                        A<Attribute>.That.Matches(
                            a => a.GetType() == typeof(EmptyCtorLessAttribute))
                        )
                    ).MustHaveHappened();
                });

                It("can be removed by specifing TAttribute", () =>
                {
                    var (builder, typeInfo) = CreateBuilderWithTypesInfo();

                    builder.WithAttribute<EmptyCtorLessAttribute>();

                    typeInfo
                        .FindAttribute<EmptyCtorLessAttribute>()
                        .ShouldNotBeNull();

                    builder.RemoveAttribute<EmptyCtorLessAttribute>();

                    typeInfo
                        .FindAttribute<EmptyCtorLessAttribute>()
                        .ShouldBeNull();
                });

                It("can be removed by specifing typeof(TAttribute)", () =>
                {
                    var (builder, typeInfo) = CreateBuilderWithTypesInfo();

                    builder.WithAttribute<EmptyCtorLessAttribute>();

                    typeInfo
                        .FindAttribute<EmptyCtorLessAttribute>()
                        .ShouldNotBeNull();

                    builder.RemoveAttribute(typeof(EmptyCtorLessAttribute));

                    typeInfo
                        .FindAttribute<EmptyCtorLessAttribute>()
                        .ShouldBeNull();
                });

                It("can be removed by specifing predicate", () =>
                {
                    var (builder, typeInfo) = CreateBuilderWithTypesInfo();

                    builder.WithAttribute<EmptyCtorLessAttribute>();

                    typeInfo
                        .FindAttribute<EmptyCtorLessAttribute>()
                        .ShouldNotBeNull();

                    builder.RemoveAttribute(t => t.GetType() == typeof(EmptyCtorLessAttribute));

                    typeInfo
                        .FindAttribute<EmptyCtorLessAttribute>()
                        .ShouldBeNull();
                });

                It("can be configured", () =>
                {
                    var value = Guid.NewGuid().ToString();
                    var (builder, typeInfo) = CreateBuilderWithTypesInfo();
                    builder.WithAttribute<EmptyCtorLessAttribute>();

                    builder.ConfigureAttribute<EmptyCtorLessAttribute>(a => a.AttributeProperty = value);

                    typeInfo
                        .FindAttribute<EmptyCtorLessAttribute>()
                        .AttributeProperty.ShouldBe(value);
                });
            });

            Describe("For Members", () =>
            {
                static (ModelBuilder<ModelBuilderTarget>, ITypeInfo, IMemberInfo) CreateBuilderWithMember()
                {
                    var typesInfo = A.Fake<ITypesInfo>();
                    var typeInfo = A.Fake<ITypeInfo>();
                    var memberInfo = A.Fake<IMemberInfo>();

                    A.CallTo(() => typesInfo.FindTypeInfo(typeof(ModelBuilderTarget)))
                        .Returns(typeInfo);

                    A.CallTo(() => typeInfo.FindMember(nameof(ModelBuilderTarget.ListProperty)))
                        .Returns(memberInfo);

                    A.CallTo(() => memberInfo.Name)
                        .Returns(nameof(ModelBuilderTarget.ListProperty));

                    var builder = ModelBuilder.Create<ModelBuilderTarget>(typesInfo);
                    return (builder, typeInfo, memberInfo);
                }

                It("can add attribute", () =>
                {
                    var (builder, _, memberInfo) = CreateBuilderWithMember();

                    builder.For(m => m.ListProperty)
                        .WithAttribute<EmptyCtorLessAttribute>();

                    A.CallTo(() =>
                        memberInfo.AddAttribute(
                            A<Attribute>.That.Matches(a =>
                                a.GetType() == typeof(EmptyCtorLessAttribute)
                            )
                    )).MustHaveHappenedOnceExactly();
                });

                It("has correct PropertyName", () =>
                {
                    var (builder, _, _) = CreateBuilderWithMember();

                    builder.For(m => m.ListProperty)
                        .PropertyName.ShouldBe(nameof(ModelBuilderTarget.ListProperty));
                });
            });
        });
    }
}
