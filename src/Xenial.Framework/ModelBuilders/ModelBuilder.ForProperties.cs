using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders
{
    public partial class ModelBuilder<TClassType>
    {
        /// <summary>   Fors the properties. </summary>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="propertyExpressions">  The property expressions. </param>
        ///
        /// <returns>
        /// Xenial.Framework.ModelBuilders.MultiplePropertyBuilder&lt;object?, TClassType&gt;.
        /// </returns>

        public AggregatedPropertyBuilder<object?, TClassType> ForProperties(params Expression<Func<TClassType, object?>>[] propertyExpressions)
        {
            _ = propertyExpressions ?? throw new ArgumentNullException(nameof(propertyExpressions));
            var propertyBuilderList = new List<IPropertyBuilder<object?, TClassType>>();

            foreach (var propertyExpression in propertyExpressions)
            {
                _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpressions));

                var propertyName = ExpressionHelper.Property(propertyExpression);
                if (propertyName is not null)
                {
                    var memberInfo = TypeInfo.FindMember(propertyName);
                    if (memberInfo is not null)
                    {
                        var propertyBuilder = PropertyBuilder.PropertyBuilderFor<object?, TClassType>(memberInfo);

                        Add(propertyBuilder);

                        propertyBuilderList.Add(propertyBuilder);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Could not create PropertyBuilder for '{TypeInfo}'.{propertyName} ");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Could not create PropertyBuilder for '{TypeInfo}'.{propertyExpression} ");
                }
            }

            return new AggregatedPropertyBuilder<object?, TClassType>(this, propertyBuilderList);
        }

        /// <summary>   Fors the properties. </summary>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <typeparam name="TPropertyType">    The type of the t property type. </typeparam>
        /// <param name="propertyExpressions">  The property expressions. </param>
        ///
        /// <returns>
        /// Xenial.Framework.ModelBuilders.MultiplePropertyBuilder&lt;TPropertyType?, TClassType&gt;.
        /// </returns>

        public AggregatedPropertyBuilder<TPropertyType?, TClassType> ForProperties<TPropertyType>(params Expression<Func<TClassType, TPropertyType?>>[] propertyExpressions)
        {
            _ = propertyExpressions ?? throw new ArgumentNullException(nameof(propertyExpressions));
            var propertyBuilderList = new List<IPropertyBuilder<TPropertyType?, TClassType>>();

            foreach (var propertyExpression in propertyExpressions)
            {
                var propertyName = ExpressionHelper.Property(propertyExpression);
                if (propertyName is not null)
                {
                    var memberInfo = TypeInfo.FindMember(propertyName);
                    if (memberInfo is not null)
                    {
                        var propertyBuilder = PropertyBuilder.PropertyBuilderFor<TPropertyType?, TClassType>(memberInfo);

                        Add(propertyBuilder);

                        propertyBuilderList.Add(propertyBuilder);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Could not create PropertyBuilder for '{TypeInfo}'.{propertyName} ");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Could not create PropertyBuilder for '{TypeInfo}'.{propertyExpression} ");
                }
            }

            return new AggregatedPropertyBuilder<TPropertyType?, TClassType>(this, propertyBuilderList);
        }

        /// <summary>   Fors the type of the properties of. </summary>
        ///
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>
        /// IAggregatedPropertyBuilder&lt;System.Nullable&lt;System.Object&gt;, TClassType&gt;.
        /// </returns>

        public IAggregatedPropertyBuilder<object?, TClassType> ForProperties(Func<IMemberInfo, bool> predicate)
        {
            var propertyBuilders = TypeInfo.Members.Where(predicate).Select(m => PropertyBuilder.PropertyBuilderFor<object?, TClassType>(m));

            foreach (var propertyBuilder in propertyBuilders)
            {
                Add(propertyBuilder);
            }

            return new AggregatedPropertyBuilder<object?, TClassType>(this, propertyBuilders);
        }
    }
}
