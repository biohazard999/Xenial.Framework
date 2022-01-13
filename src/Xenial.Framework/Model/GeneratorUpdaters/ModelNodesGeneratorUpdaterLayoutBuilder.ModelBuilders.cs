﻿using System;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.ModelBuilders;

public static partial class ModelBuilderExtensions
{
    /// <summary>   Withes the detail view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="layoutFunctor">    The layout functor. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithDetailViewLayout<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        BuildLayoutFunctor layoutFunctor
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = layoutFunctor ?? throw new ArgumentNullException(nameof(layoutFunctor));
        return modelBuilder.WithAttribute(new DetailViewLayoutBuilderAttribute(layoutFunctor));
    }

    /// <summary>   Withes the detail view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="viewId">           The view id. </param>
    /// <param name="layoutFunctor">    The layout functor. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithDetailViewLayout<TClassType>(
        this IModelBuilder<TClassType> modelBuilder,
        string viewId,
        BuildLayoutFunctor layoutFunctor
    )
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = layoutFunctor ?? throw new ArgumentNullException(nameof(layoutFunctor));
        _ = viewId ?? throw new ArgumentNullException(nameof(viewId));
        return modelBuilder.WithAttribute(new DetailViewLayoutBuilderAttribute(layoutFunctor)
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
    /// <param name="layoutBuilder">    The layout builder. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithDetailViewLayout<TClassType>(
      this IModelBuilder<TClassType> modelBuilder,
      Func<LayoutBuilder<TClassType>, Layout> layoutBuilder
    )
        where TClassType : class
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = layoutBuilder ?? throw new ArgumentNullException(nameof(layoutBuilder));

        var layout = layoutBuilder(new LayoutBuilder<TClassType>());

        return modelBuilder.WithAttribute(new DetailViewLayoutBuilderAttribute(() => layout));
    }


    /// <summary>   Withes the detail view layout. </summary>
    ///
    /// <exception cref="ArgumentNullException">    modelBuilder. </exception>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="modelBuilder">     The model builder. </param>
    /// <param name="viewId">           The view id. </param>
    /// <param name="layoutBuilder">    The layout builder. </param>
    ///
    /// <returns>   IModelBuilder&lt;TClassType&gt;. </returns>

    public static IModelBuilder<TClassType> WithDetailViewLayout<TClassType>(
      this IModelBuilder<TClassType> modelBuilder,
      string viewId,
      Func<LayoutBuilder<TClassType>, Layout> layoutBuilder
    )
        where TClassType : class
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _ = viewId ?? throw new ArgumentNullException(nameof(viewId));
        _ = layoutBuilder ?? throw new ArgumentNullException(nameof(layoutBuilder));

        var layout = layoutBuilder(new LayoutBuilder<TClassType>());

        return modelBuilder.WithAttribute(new DetailViewLayoutBuilderAttribute(() => layout)
        {
            ViewId = viewId
        });
    }
}
