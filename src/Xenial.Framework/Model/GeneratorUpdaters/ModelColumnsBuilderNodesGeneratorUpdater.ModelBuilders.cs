using System;

using Xenial.Framework.Layouts;

namespace Xenial.Framework.ModelBuilders;

public static partial class ModelBuilderExtensions
{
    #region ListView

    /// <summary>   Withes the list view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="columnsFunctor">   The columns functor. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithListViewColumns<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        BuildColumnsFunctor columnsFunctor
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = columnsFunctor ?? throw new ArgumentNullException(nameof(columnsFunctor));
        return modelBuilder.WithAttribute(new ListViewColumnsBuilderAttribute(columnsFunctor));
    }

    /// <summary>   Withes the list view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="viewId">           The view id. </param>
    /// <param name="columnsFunctor">   The columns functor. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithListViewColumns<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        string viewId,
        BuildColumnsFunctor columnsFunctor
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(viewId));
        _ = columnsFunctor ?? throw new ArgumentNullException(nameof(columnsFunctor));
        return modelBuilder.WithAttribute(new ListViewColumnsBuilderAttribute(columnsFunctor)
        {
            ViewId = viewId
        });
    }

    /// <summary>   Withes the detail view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="columnsBuilder">   The layout builder. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithListViewColumns<TClassType>(
      this IModelBuilder<TClassType> modelBuilder,
      Func<ColumnsBuilder<TClassType>, Columns> columnsBuilder
    )
        where TClassType : class
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = columnsBuilder ?? throw new ArgumentNullException(nameof(columnsBuilder));

        var columns = columnsBuilder(new ColumnsBuilder<TClassType>());

        return modelBuilder.WithAttribute(new ListViewColumnsBuilderAttribute(() => columns));
    }

    /// <summary>   Withes the detail view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="viewId">           The view id. </param>
    /// <param name="columnsBuilder">   The layout builder. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithListViewColumns<TClassType>(
      this IModelBuilder<TClassType> modelBuilder,
      string viewId,
      Func<ColumnsBuilder<TClassType>, Columns> columnsBuilder
    )
        where TClassType : class
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(viewId));
        _ = columnsBuilder ?? throw new ArgumentNullException(nameof(columnsBuilder));

        var columns = columnsBuilder(new ColumnsBuilder<TClassType>());

        return modelBuilder.WithAttribute(new ListViewColumnsBuilderAttribute(() => columns)
        {
            ViewId = viewId
        });
    }
    #endregion

    #region LookupListView

    /// <summary>   Withes the list view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="columnsFunctor">   The columns functor. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithLookupListViewColumns<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        BuildColumnsFunctor columnsFunctor
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = columnsFunctor ?? throw new ArgumentNullException(nameof(columnsFunctor));
        return modelBuilder.WithAttribute(new LookupListViewColumnsBuilderAttribute(columnsFunctor));
    }

    /// <summary>   Withes the list view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="viewId">           The view id. </param>
    /// <param name="columnsFunctor">   The columns functor. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithLookupListViewColumns<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        string viewId,
        BuildColumnsFunctor columnsFunctor
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(viewId));
        _ = columnsFunctor ?? throw new ArgumentNullException(nameof(columnsFunctor));
        return modelBuilder.WithAttribute(new LookupListViewColumnsBuilderAttribute(columnsFunctor)
        {
            ViewId = viewId
        });
    }

    /// <summary>   Withes the detail view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="columnsBuilder">   The layout builder. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithLookupListViewColumns<TClassType>(
      this IModelBuilder<TClassType> modelBuilder,
      Func<ColumnsBuilder<TClassType>, Columns> columnsBuilder
    )
        where TClassType : class
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = columnsBuilder ?? throw new ArgumentNullException(nameof(columnsBuilder));

        var columns = columnsBuilder(new ColumnsBuilder<TClassType>());

        return modelBuilder.WithAttribute(new LookupListViewColumnsBuilderAttribute(() => columns));
    }

    /// <summary>   Withes the detail view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="viewId">           The view id. </param>
    /// <param name="columnsBuilder">   The layout builder. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithLookupListViewColumns<TClassType>(
      this IModelBuilder<TClassType> modelBuilder,
      string viewId,
      Func<ColumnsBuilder<TClassType>, Columns> columnsBuilder
    )
        where TClassType : class
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(viewId));
        _ = columnsBuilder ?? throw new ArgumentNullException(nameof(columnsBuilder));

        var columns = columnsBuilder(new ColumnsBuilder<TClassType>());

        return modelBuilder.WithAttribute(new LookupListViewColumnsBuilderAttribute(() => columns)
        {
            ViewId = viewId
        });
    }

    #endregion
}
