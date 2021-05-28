using DevExpress.ExpressApp.DC;

using Shouldly;

using Xenial.Framework.ModelBuilders;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.ModelBuilders
{
    /// <summary>   A property builder facts. </summary>
    public static class PropertyBuilderFacts
    {
        /// <summary>   Property builder tests. </summary>
        public static void PropertyBuilderTests() => Describe(nameof(PropertyBuilder), () =>
        {
            IModelBuilder<ModelBuilderTarget> CreateBuilder() => ModelBuilder.Create<ModelBuilderTarget>(new TypesInfo());
            It("adds attribute", () =>
            {
                var builder = CreateBuilder();
                builder
                    .For(p => p.StringProperty)
                    .WithAttribute<EmptyCtorLessAttribute>();

                builder.For(p => p.StringProperty)
                    .MemberInfo
                    .FindAttribute<EmptyCtorLessAttribute>()
                    .ShouldNotBeNull();
            });

            It("removes attribute", () =>
            {
                var builder = CreateBuilder();
                builder
                    .For(p => p.StringProperty)
                    .WithAttribute<EmptyCtorLessAttribute>();

                builder
                    .For(p => p.StringProperty)
                    .RemoveAttribute<EmptyCtorLessAttribute>();

                builder.For(p => p.StringProperty)
                    .MemberInfo
                    .FindAttribute<EmptyCtorLessAttribute>()
                    .ShouldBeNull();
            });
        });
    }
}
