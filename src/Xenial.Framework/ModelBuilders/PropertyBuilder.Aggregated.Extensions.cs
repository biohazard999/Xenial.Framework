using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>   Class AggregatedPropertyBuilderExtensions. </summary>
    public static class AggregatedPropertyBuilderExtensions
    {
        /// <summary>   Excepts the specified builder. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="TPropertyType">    The type of the t property type. </typeparam>
        /// <typeparam name="TClassType">       The type of the t class type. </typeparam>
        /// <param name="builder">              The builder. </param>
        /// <param name="propertyExpressions">  The property expressions. </param>
        ///
        /// <returns>
        /// Xenial.Framework.ModelBuilders.IAggregatedPropertyBuilder&lt;TPropertyType, TClassType&gt;.
        /// </returns>

        public static IAggregatedPropertyBuilder<TPropertyType, TClassType> Except<TPropertyType, TClassType>(
            this IAggregatedPropertyBuilder<TPropertyType, TClassType> builder,
            params Expression<Func<TClassType, TPropertyType?>>[] propertyExpressions
        )
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            var propertyBuilders = builder.PropertyBuilders.ToList();
            foreach (var propertyExpression in propertyExpressions)
            {
                _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpressions));

                var propertyName = builder.ModelBuilder.ExpressionHelper.Property(propertyExpression);
                if (propertyName is not null)
                {
                    var memberInfo = builder.ModelBuilder.TypeInfo.FindMember(propertyName);
                    if (memberInfo is not null)
                    {
                        var propertyBuilder = propertyBuilders.FirstOrDefault(m => m.MemberInfo == memberInfo);
                        if (propertyBuilder is not null)
                        {
                            propertyBuilders.Remove(propertyBuilder);
                        }
                    }
                }
            }

            return new AggregatedPropertyBuilder<TPropertyType, TClassType>(builder.ModelBuilder, propertyBuilders);
        }

        /// <summary>   Excepts the specified builder. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="TPropertyType">    The type of the t property type. </typeparam>
        /// <typeparam name="TClassType">       The type of the t class type. </typeparam>
        /// <param name="builder">      The builder. </param>
        /// <param name="exceptFilter"> The except filter. </param>
        ///
        /// <returns>
        /// Xenial.Framework.ModelBuilders.IAggregatedPropertyBuilder&lt;TPropertyType, TClassType&gt;.
        /// </returns>

        public static IAggregatedPropertyBuilder<TPropertyType, TClassType> Except<TPropertyType, TClassType>(
            this IAggregatedPropertyBuilder<TPropertyType, TClassType> builder,
            Func<IEnumerable<IPropertyBuilder<TPropertyType, TClassType>>, IEnumerable<IPropertyBuilder<TPropertyType, TClassType>>> exceptFilter
        )
        {
            _ = builder ?? throw new ArgumentNullException(nameof(exceptFilter));
            _ = exceptFilter ?? throw new ArgumentNullException(nameof(builder));

            var propertyBuilders = exceptFilter(builder.PropertyBuilders);
            propertyBuilders = builder.PropertyBuilders.Except(propertyBuilders);
            return new AggregatedPropertyBuilder<TPropertyType, TClassType>(builder.ModelBuilder, propertyBuilders);
        }
    }
}
