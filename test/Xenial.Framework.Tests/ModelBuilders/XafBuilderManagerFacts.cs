using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;

using FakeItEasy;

using Xenial.Framework.ModelBuilders;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.ModelBuilders
{
    public static class XafBuilderManagerFacts
    {
        public static void XafBuilderManagerTests() => Describe(nameof(XafBuilderManager), () =>
        {
            It("calls Refresh after Build", () =>
            {
                var typesInfo = A.Fake<ITypesInfo>();

                IBuilderManager builderManager = new XafBuilderManager(typesInfo);
                var builderA = A.Fake<IBuilder>(o => o.Implements<ITypeInfoProvider>());
                var builderB = A.Fake<IBuilder>(o => o.Implements<ITypeInfoProvider>());

                var typeInfoA = A.Fake<ITypeInfo>();
                var typeInfoB = A.Fake<ITypeInfo>();

                A.CallTo(() => ((ITypeInfoProvider)builderA).TypeInfo).Returns(typeInfoA);
                A.CallTo(() => ((ITypeInfoProvider)builderB).TypeInfo).Returns(typeInfoB);

                builderManager
                    .Add(builderA)
                    .Add(builderB)
                    .Build();

                A.CallTo(
                    () => builderA.Build()
                ).MustHaveHappenedOnceExactly()
                .Then(
                    A.CallTo(() => typesInfo.RefreshInfo(typeInfoA)
                ).MustHaveHappenedOnceExactly())
                .Then(
                    A.CallTo(() => builderB.Build()
                ).MustHaveHappenedOnceExactly())
                .Then(
                    A.CallTo(() => typesInfo.RefreshInfo(typeInfoB)
                ).MustHaveHappenedOnceExactly());
            });

            It("uses GetBuilders overload", () =>
            {
                var sut = new TestXafBuilderManager(A.Fake<ITypesInfo>());

                sut.Build();

                A.CallTo(
                    () => sut.Builder.Build()
                ).MustHaveHappened();
            });
        });

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "By design")]
        public class TestXafBuilderManager : XafBuilderManager
        {
            internal TestXafBuilderManager(ITypesInfo typesInfo) : base(typesInfo) { }

            public IBuilder Builder { get; } = A.Fake<IBuilder>();

            protected override IEnumerable<IBuilder> GetBuilders() => new[]
            {
                Builder
            };
        }
    }
}
