﻿using System;
using System.ComponentModel;

using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class PropertyBuilderExtensions
    {
        /// <summary>   Usings the property editor. </summary>
        ///
        /// <typeparam name="TProperty">    The type of the property. </typeparam>
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">                  The builder. </param>
        /// <param name="propertyEditorTypeName">   Name of the property editor type. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

        public static IPropertyBuilder<TProperty?, TClassType> UsingPropertyEditor<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string propertyEditorTypeName)
            => builder.WithModelDefault(ModelDefaults.PropertyEditorType, propertyEditorTypeName);

        /// <summary>   Usings the property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="TProperty">    The type of the property. </typeparam>
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">              The builder. </param>
        /// <param name="propertyEditorType">   Type of the property editor. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

        public static IPropertyBuilder<TProperty?, TClassType> UsingPropertyEditor<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, Type propertyEditorType)
        {
            _ = propertyEditorType ?? throw new ArgumentNullException(nameof(propertyEditorType));
            return builder.UsingPropertyEditor(propertyEditorType.FullName ?? string.Empty);
        }

        /// <summary>   Class ModelBuilderPropertyEditorOptions. </summary>
#pragma warning disable CA1034 //Do not nest types: By Design
        public sealed class ModelBuilderPropertyEditorOptions
        {
            internal ModelBuilderPropertyEditorOptions() { }

            /// <summary>   Gets or sets the type of the property editor. </summary>
            ///
            /// <value> The type of the property editor. </value>

            [EditorBrowsable(EditorBrowsableState.Never)]
            public Type? PropertyEditorType { get; set; }

            /// <summary>   Specifies the PropertyEditorType. </summary>
            ///
            /// <typeparam name="TEditor">  The type of the t editor. </typeparam>
            ///
            /// <returns>   ModelBuilderPropertyEditorOptions. </returns>

            public ModelBuilderPropertyEditorOptions Editor<TEditor>()
                where TEditor : PropertyEditor
            {
                PropertyEditorType = typeof(TEditor);
                return this;
            }
        }
#pragma warning restore CA1034 //Do not nest types: By Design

        /// <summary>   Usings the property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <typeparam name="TProperty">    The type of the t property. </typeparam>
        /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
        /// <param name="builder">  The builder. </param>
        /// <param name="options">  The options. </param>
        ///
        /// <returns>
        /// Xenial.Framework.ModelBuilders.IPropertyBuilder&lt;TProperty?, TClassType&gt;.
        /// </returns>

        public static IPropertyBuilder<TProperty?, TClassType> UsingPropertyEditor<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, Action<ModelBuilderPropertyEditorOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            var o = new ModelBuilderPropertyEditorOptions();
            options(o);
            _ = o.PropertyEditorType ?? throw new InvalidOperationException($"You need to specify the {nameof(o.PropertyEditorType)} by calling the options.Editor<T> method");

            return builder.UsingPropertyEditor(o.PropertyEditorType.FullName ?? string.Empty);
        }

        /// <summary>   Usings the editor alias. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="TProperty">    The type of the property. </typeparam>
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">      The builder. </param>
        /// <param name="editorAlias">  The editor alias. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

        public static IPropertyBuilder<TProperty?, TClassType> UsingEditorAlias<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string editorAlias)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new EditorAliasAttribute(editorAlias));
        }

    }
}
