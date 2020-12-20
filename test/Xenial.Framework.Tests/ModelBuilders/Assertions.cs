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

        public static IModelBuilder AssertAttribute<TAttribute>(this IModelBuilder builder, Func<TAttribute, bool> assertion)
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

        public static IModelBuilder AssertHasAttribute<TAttribute>(this IModelBuilder builder)
               where TAttribute : Attribute
        {
            var attr = builder.TypeInfo.FindAttribute<TAttribute>();

            attr.ShouldNotBeNull();

            return builder;
        }

        public static IPropertyBuilder<TProperty, TClass> AssertModelDefaultAttribute<TProperty, TClass>(this IPropertyBuilder<TProperty, TClass> builder, string propertyName, string propertyValue)
        {
            var attr = builder.MemberInfo.FindAttributes<ModelDefaultAttribute>().FirstOrDefault(a => a.PropertyName == propertyName);

            attr.ShouldSatisfyAllConditions
            (
                () => attr.ShouldNotBeNull(),
                () => attr!.PropertyName.ShouldBe(propertyName),
                () => attr!.PropertyValue.ShouldBe(propertyValue)
            );

            return builder;
        }

        public static IPropertyBuilder AssertAttribute<TAttribute, TProperty>(this IPropertyBuilder builder, Func<TAttribute, bool> assertion)
               where TAttribute : Attribute
        {
            var attr = builder.MemberInfo.FindAttribute<TAttribute>();

            attr.ShouldSatisfyAllConditions
            (
                () => attr.ShouldNotBeNull(),
                () => assertion.Invoke(attr).ShouldBe(true)
            );

            return builder;
        }

        public static IPropertyBuilder AssertHasAttribute<TAttribute>(this IPropertyBuilder builder)
            where TAttribute : Attribute
        {
            var attr = builder.MemberInfo.FindAttribute<TAttribute>();

            attr.ShouldNotBeNull();

            return builder;
        }
    }
}
