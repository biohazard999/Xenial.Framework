
namespace Xenial.Framework.ModelBuilders
{
    public static partial class PropertyBuilderExtensions
    {
        /// <summary>   Determines whether this instance is password. </summary>
        ///
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">  The builder. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;string?,TClassType&gt; </returns>

        public static IPropertyBuilder<string?, TClassType> IsPassword<TClassType>(this IPropertyBuilder<string?, TClassType> builder)
            => builder.WithModelDefault(ModelDefaults.IsPassword, true);

        /// <summary>   Withes the predefined values. </summary>
        ///
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">  The builder. </param>
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;string?,TClassType&gt; </returns>

        public static IPropertyBuilder<string?, TClassType> WithPredefinedValues<TClassType>(this IPropertyBuilder<string?, TClassType> builder, string value)
            => builder.WithModelDefault(ModelDefaults.PredefinedValues, value);

        /// <summary>   Withes the predefined values. </summary>
        ///
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">  The builder. </param>
        /// <param name="values">   The values. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;string?,TClassType&gt; </returns>

        public static IPropertyBuilder<string?, TClassType> WithPredefinedValues<TClassType>(this IPropertyBuilder<string?, TClassType> builder, params object[] values)
            => builder.WithPredefinedValues(string.Join(";", values));
    }
}
