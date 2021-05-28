using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class ModelBuilder
    {
        /// <summary>
        /// Gets or sets a value indicating whether [include nullable types in for properties queries].
        /// </summary>
        ///
        /// <value>
        /// <c>true</c> if [include nullable types in for properties queries]; otherwise, <c>false</c>.
        /// </value>

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool IncludeNullableTypesInForPropertiesQueries { get; set; } = true;
    }

    public partial class ModelBuilder<TClassType>
    {
        /// <summary>
        /// Gets a value indicating whether [include nullable types in for properties queries].
        /// </summary>
        ///
        /// <value>
        /// <c>true</c> if [include nullable types in for properties queries]; otherwise, <c>false</c>.
        /// </value>

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool IncludeNullableTypesInForPropertiesQueries => ModelBuilder.IncludeNullableTypesInForPropertiesQueries;

        /// <summary>   Fors all properties. </summary>
        ///
        /// <typeparam name="TPropertyType">    Type of the property type. </typeparam>
        /// <param name="includeNullableTypes"> (Optional) List of types of the include nullables. </param>
        ///
        /// <returns>
        /// IAggregatedPropertyBuilder&lt;System.Nullable&lt;System.Object&gt;, TClassType&gt;.
        /// </returns>

        public IAggregatedPropertyBuilder<TPropertyType?, TClassType> ForPropertiesOfType<TPropertyType>(bool? includeNullableTypes = null)
        {
            if (!includeNullableTypes.HasValue)
            {
                includeNullableTypes = IncludeNullableTypesInForPropertiesQueries;
            }

            var propertyBuilders = TypeInfo.Members.Where(m =>
            {
                if (typeof(TPropertyType).IsAssignableFrom(m.MemberType))
                {
                    return true;
                }
                if (includeNullableTypes.HasValue && includeNullableTypes.Value == true)
                {
                    if (m.MemberTypeInfo.IsNullable)
                    {
                        if (typeof(TPropertyType).IsAssignableFrom(m.MemberTypeInfo.UnderlyingTypeInfo.Type))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }).Select(m => PropertyBuilder.PropertyBuilderFor<TPropertyType?, TClassType>(m));

            foreach (var propertyBuilder in propertyBuilders)
            {
                Add(propertyBuilder);
            }

            return new AggregatedPropertyBuilder<TPropertyType?, TClassType>(this, propertyBuilders);
        }
    }
}
