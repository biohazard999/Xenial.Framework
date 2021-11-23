using System;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders;

public static partial class PropertyBuilderExtensions
{
    /// <summary>   Immediates the posts data. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> ImmediatePostsData<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute<ImmediatePostDataAttribute>();
    }

    /// <summary>   Allowings the edit. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> AllowingEdit<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder.WithModelDefault(ModelDefaults.AllowEdit, true);

    /// <summary>   Nots the allowing edit. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> NotAllowingEdit<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder.WithModelDefault(ModelDefaults.AllowEdit, false);

    /// <summary>   Allowings the new. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> AllowingNew<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder.WithModelDefault(ModelDefaults.AllowNew, true);

    /// <summary>   Nots the allowing new. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> NotAllowingNew<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder.WithModelDefault(ModelDefaults.AllowNew, false);

    /// <summary>   Allowings the delete. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> AllowingDelete<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder.WithModelDefault(ModelDefaults.AllowDelete, true);

    /// <summary>   Nots the allowing delete. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> NotAllowingDelete<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder.WithModelDefault(ModelDefaults.AllowDelete, false);

    /// <summary>   Allowings the everything. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> AllowingEverything<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder
            .AllowingDelete()
            .AllowingEdit()
            .AllowingNew();

    /// <summary>   Allowings the nothing. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> AllowingNothing<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder
            .NotAllowingDelete()
            .NotAllowingEdit()
            .NotAllowingNew();
}
