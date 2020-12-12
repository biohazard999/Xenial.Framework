using System;

using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class PropertyBuilderExtensions
    {
        /// <summary>
        /// Withes the model default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns></returns>
        public static IPropertyBuilder<T, TType> WithModelDefault<T, TType>(this IPropertyBuilder<T, TType> builder, string propertyName, string propertyValue)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new ModelDefaultAttribute(propertyName, propertyValue));
        }

        /// <summary>
        /// Adds an ModelDefaultAttribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">if set to <c>true</c> [property value].</param>
        /// <returns></returns>
        public static IPropertyBuilder<T, TType> WithModelDefault<T, TType>(this IPropertyBuilder<T, TType> builder, string propertyName, bool propertyValue)
            => builder.WithModelDefault(propertyName, propertyValue.ToString());
    }
}
