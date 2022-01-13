using System;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders;

/// <summary>   Class TokenEditorsPropertyBuilderExtensions. </summary>
public static class StrepProgressPropertyBuilderExtensions
{
    /// <summary>
    /// Use the Step Progress Enum Property Editor <see cref="StepProgressEnumEditorAttribute" />
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException">    builder. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TProperty&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<TProperty, TClassType> UseStepProgressEnumPropertyEditor<TProperty, TClassType>(this IPropertyBuilder<TProperty, TClassType> builder)
        where TProperty : Enum
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new StepProgressEnumEditorAttribute());
    }

    /// <summary>   Uses the step progress enum property editor. </summary>
    ///
    /// <exception cref="ArgumentNullException">    builder. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TProperty&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<TProperty?, TClassType> UseStepProgressEnumPropertyEditor<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        where TProperty : struct, Enum
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new StepProgressEnumEditorAttribute());
    }
}
