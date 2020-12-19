using System;
using System.Linq;

using DevExpress.ExpressApp.Model;

using Shouldly;

using Xenial.Framework.ModelBuilders;

namespace Xenial.Framework.Tests.ModelBuilders
{
    internal static class Assertions
    {
        public static IModelBuilder<T> AssertModelDefaultAttribute<T>(this IModelBuilder<T> builder, string propertyName, string propertyValue)
        {
            var attr = builder.TypeInfo.FindAttributes<ModelDefaultAttribute>().FirstOrDefault(a => a.PropertyName == propertyName);

            attr.ShouldSatisfyAllConditions
            (
                () => attr.ShouldNotBeNull(),
                () => attr!.PropertyName.ShouldBe(propertyName),
                () => attr!.PropertyValue.ShouldBe(propertyValue)
            );
            return builder;
        }

        public static IModelBuilder<TClassType> AssertAttribute<TAttribute, TClassType>(this IModelBuilder<TClassType> builder, Func<TAttribute, bool> assertion)
               where TAttribute : Attribute
        {
            var attr = builder.TypeInfo.FindAttribute<TAttribute>();

            attr.ShouldSatisfyAllConditions
            (
                () => attr.ShouldNotBeNull(),
                () => assertion.Invoke(attr).ShouldBe(true)
            );
            return builder;
        }

    }
}
