using FakeItEasy;

using Xenial.Framework.ModelBuilders;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.ModelBuilders
{
    public static class BuilderManagerFacts
    {
        public static void BuilderManagerTests() => Describe(nameof(BuilderManager), () =>
        {
            It("calls first builder", () =>
            {
                IBuilderManager builderManager = new BuilderManager();
                var builder = A.Fake<IBuilder>();

                builderManager
                    .Add(builder)
                    .Build();

                A.CallTo(
                    () => builder.Build()
                ).MustHaveHappenedOnceExactly();
            });

            It("calls builders in order", () =>
            {
                IBuilderManager builderManager = new BuilderManager();
                var builderA = A.Fake<IBuilder>();
                var builderB = A.Fake<IBuilder>();

                builderManager
                    .Add(builderA)
                    .Add(builderB)
                    .Build();

                A.CallTo(
                    () => builderA.Build()
                ).MustHaveHappenedOnceExactly()
                .Then(
                    A.CallTo(() => builderB.Build()
                ).MustHaveHappenedOnceExactly());
            });
        });
    }
}
