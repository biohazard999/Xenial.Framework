using System;
using System.Linq.Expressions;

using Shouldly;

using static Xenial.Tasty;

#nullable disable

namespace Xenial.Utils.Tests
{
    /// <summary>   An expression helper facts. </summary>
    public static class ExpressionHelperFacts
    {
        private class TargetClass
        {
            /// <summary>   Gets or sets a. </summary>
            ///
            /// <value> a. </value>

            public TargetClass A { get; set; }

            /// <summary>   Gets or sets the b. </summary>
            ///
            /// <value> The b. </value>

            public TargetClass B { get; set; }

            /// <summary>   Gets or sets the c. </summary>
            ///
            /// <value> The c. </value>

            public TargetClass C { get; set; }
        }

        /// <summary>   Expression helper tests. </summary>
        public static void ExpressionHelperTests() => Describe(nameof(ExpressionHelper), () =>
        {
            _ = new TargetClass(); //Fix CA1812

            static string PropertyName<TRet>(Expression<Func<TargetClass, TRet>> expression)
                => ExpressionHelper.GetPropertyPath(expression);

            It("simple path A",
                () => PropertyName(m => m.A).ShouldBe("A")
            );

            It("simple path B",
                () => PropertyName(m => m.B).ShouldBe("B")
            );

            It("simple path C",
                () => PropertyName(m => m.C).ShouldBe("C")
            );

            It("compley path 1",
                () => PropertyName(m => m.A.A.A.B.C.A).ShouldBe("A.A.A.B.C.A")
            );

            It("compley path 2",
                () => PropertyName(m => m.C.A.B).ShouldBe("C.A.B")
            );
        });
    }
}
