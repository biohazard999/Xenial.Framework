using System;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class PropertyBuilderExtensions
    {
        /// <summary>
        /// Determines whether [is visible in detail view] [the specified builder].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsVisibleInDetailView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new VisibleInDetailViewAttribute(true));
        }

        /// <summary>
        /// Determines whether [is not visible in detail view] [the specified builder].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsNotVisibleInDetailView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new VisibleInDetailViewAttribute(false));
        }

        /// <summary>
        /// Determines whether [is visible in ListView].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsVisibleInListView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new VisibleInListViewAttribute(true));
        }

        /// <summary>
        /// Determines whether [is not visible in ListView].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsNotVisibleInListView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new VisibleInListViewAttribute(false));
        }

        /// <summary>
        /// Determines whether [is visible in lookup ListView].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsVisibleInLookupListView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new VisibleInLookupListViewAttribute(true));
        }

        /// <summary>
        /// Determines whether [is not visible in lookup ListView].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsNotVisibleInLookupListView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new VisibleInLookupListViewAttribute(false));
        }

        /// <summary>
        /// Determines whether [is visible in any view].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsVisibleInAnyView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder
                .IsVisibleInDetailView()
                .IsVisibleInListView()
                .IsVisibleInLookupListView();

        /// <summary>
        /// Determines whether [is not visible in any view].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> IsNotVisibleInAnyView<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder
                .IsNotVisibleInDetailView()
                .IsNotVisibleInListView()
                .IsNotVisibleInLookupListView();
    }
}
