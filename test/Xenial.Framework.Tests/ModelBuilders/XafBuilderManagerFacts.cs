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
    /// <summary>   An xaf builder manager facts. </summary>
    public static class XafBuilderManagerFacts
    {
        /// <summary>   Xaf builder manager tests. </summary>
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

        /// <summary>   Manager for test xaf builders. </summary>
        ///
        /// <seealso cref="XafBuilderManager"/>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "By design")]
        public class TestXafBuilderManager : XafBuilderManager
        {
            internal TestXafBuilderManager(ITypesInfo typesInfo) : base(typesInfo) { }

            /// <summary>   Gets the builder. </summary>
            ///
            /// <value> The builder. </value>

            public IBuilder Builder { get; } = A.Fake<IBuilder>();

            /// <summary>   Gets the builders. </summary>
            ///
            /// <returns>
            /// An enumerator that allows foreach to be used to process the builders in this collection.
            /// </returns>
            ///
            /// <seealso cref="Xenial.Framework.ModelBuilders.BuilderManager.GetBuilders()"/>

            protected override IEnumerable<IBuilder> GetBuilders() => new[]
            {
                Builder
            };
        }
    }
}
