using System;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class PropertyBuilderExtensions
    {
        /// <summary>
        /// Determines whether the specified caption has caption.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="caption">The caption.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> HasCaption<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder, string caption)
            => builder.WithModelDefault(ModelDefaults.Caption, caption);

        /// <summary>
        /// Determines whether the specified tooltip has tooltip.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> HasTooltip<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder, string tooltip)
            => builder.WithModelDefault(ModelDefaults.ToolTip, tooltip);

        /// <summary>
        /// Determines whether [has display format] [the specified display format].
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="displayFormat">The display format.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> HasDisplayFormat<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder, string displayFormat)
            => builder.WithModelDefault(ModelDefaults.DisplayFormat, displayFormat);

        /// <summary>
        /// Determines whether the specified index has index.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static IPropertyBuilder<TProperty, TType> HasIndex<TProperty, TType>(this IPropertyBuilder<TProperty, TType> builder, int index)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new IndexAttribute(index));
        }
    }
}
