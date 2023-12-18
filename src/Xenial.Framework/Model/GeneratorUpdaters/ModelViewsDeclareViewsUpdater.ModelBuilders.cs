using System;

using Xenial.Framework.Base;

namespace Xenial.Framework.ModelBuilders;

public static partial class ModelBuilderExtensions
{
    /// <summary>   Declares a detail view. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder"> The model builder. </param>
    /// <param name="viewId"> The view Id. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> DeclareDetailView<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        string viewId
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(modelBuilder));
        return modelBuilder.WithAttribute(new DeclareDetailViewAttribute(viewId));
    }

    /// <summary>   Declares a list view. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder"> The model builder. </param>
    /// <param name="viewId"> The view Id. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> DeclareListView<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        string viewId
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(modelBuilder));
        return modelBuilder.WithAttribute(new DeclareListViewAttribute(viewId));
    }

    /// <summary>   Declares a dashboard view. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder"> The model builder. </param>
    /// <param name="viewId"> The view Id. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> DeclareDashboardView<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        string viewId
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(modelBuilder));
        return modelBuilder.WithAttribute(new DeclareDashboardViewAttribute(viewId));
    }
}
