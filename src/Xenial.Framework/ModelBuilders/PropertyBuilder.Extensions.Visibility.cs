using System;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders;

public static partial class PropertyBuilderExtensions
{
    /// <summary>   Determines whether [is visible in detail view] [the specified builder]. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsVisibleInDetailView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new VisibleInDetailViewAttribute(true));
    }

    /// <summary>
    /// Determines whether [is not visible in detail view] [the specified builder].
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsNotVisibleInDetailView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new VisibleInDetailViewAttribute(false));
    }

    /// <summary>   Determines whether [is visible in ListView]. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsVisibleInListView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new VisibleInListViewAttribute(true));
    }

    /// <summary>   Determines whether [is not visible in ListView]. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsNotVisibleInListView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new VisibleInListViewAttribute(false));
    }

    /// <summary>   Determines whether [is visible in lookup ListView]. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsVisibleInLookupListView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new VisibleInLookupListViewAttribute(true));
    }

    /// <summary>   Determines whether [is not visible in lookup ListView]. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsNotVisibleInLookupListView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new VisibleInLookupListViewAttribute(false));
    }

    /// <summary>   Determines whether [is visible in any view]. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsVisibleInAnyView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder
            .IsVisibleInDetailView()
            .IsVisibleInListView()
            .IsVisibleInLookupListView();

    /// <summary>   Determines whether [is not visible in any view]. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> IsNotVisibleInAnyView<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
        => builder
            .IsNotVisibleInDetailView()
            .IsNotVisibleInListView()
            .IsNotVisibleInLookupListView();
}
