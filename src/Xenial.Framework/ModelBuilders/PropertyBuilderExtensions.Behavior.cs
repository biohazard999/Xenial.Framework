using System;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class PropertyBuilderExtensions
    {

        /// <summary>
        /// Immediates the posts data.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> ImmediatePostsData<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute<ImmediatePostDataAttribute>();
        }

        /// <summary>
        /// Allowings the edit.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> AllowingEdit<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder.WithModelDefault(ModelDefaults.AllowEdit, true);

        /// <summary>
        /// Nots the allowing edit.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> NotAllowingEdit<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder.WithModelDefault(ModelDefaults.AllowEdit, false);

        /// <summary>
        /// Allowings the new.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> AllowingNew<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder.WithModelDefault(ModelDefaults.AllowNew, true);

        /// <summary>
        /// Nots the allowing new.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> NotAllowingNew<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder.WithModelDefault(ModelDefaults.AllowNew, false);

        /// <summary>
        /// Allowings the delete.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> AllowingDelete<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder.WithModelDefault(ModelDefaults.AllowDelete, true);

        /// <summary>
        /// Nots the allowing delete.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> NotAllowingDelete<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder.WithModelDefault(ModelDefaults.AllowDelete, false);

        /// <summary>
        /// Allowings the everything.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> AllowingEverything<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder
                .AllowingDelete()
                .AllowingEdit()
                .AllowingNew();

        /// <summary>
        /// Allowings the nothing.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> AllowingNothing<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder)
            => builder
                .NotAllowingDelete()
                .NotAllowingEdit()
                .NotAllowingNew();
    }
}
