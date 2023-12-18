using System;

using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders;

/// <summary>
/// 
/// </summary>
public static partial class PropertyBuilderExtensions
{
    /// <summary>   Withes the model default. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    . </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">          The builder. </param>
    /// <param name="propertyName">     Name of the property. </param>
    /// <param name="propertyValue">    The property value. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> WithModelDefault<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string propertyName, string propertyValue)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new ModelDefaultAttribute(propertyName, propertyValue));
    }

    /// <summary>   Adds an ModelDefaultAttribute. </summary>
    ///
    /// <typeparam name="TProperty">    . </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">          The builder. </param>
    /// <param name="propertyName">     Name of the property. </param>
    /// <param name="propertyValue">    if set to <c>true</c> [property value]. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> WithModelDefault<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string propertyName, bool propertyValue)
        => builder.WithModelDefault(propertyName, propertyValue.ToString());
}
