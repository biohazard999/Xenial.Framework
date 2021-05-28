using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>   Class TokenEditorsPropertyBuilderExtensions. </summary>
    public static class TokenEditorsPropertyBuilderExtensions
    {
        /// <summary>
        /// Use the Token String Property Editor <see cref="TokenStringEditorAttribute"/>
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">  The builder. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;string?,TClassType&gt; </returns>

        public static IPropertyBuilder<string?, TClassType> UseTokenStringPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new TokenStringEditorAttribute());
        }

        /// <summary>   Uses the token string property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    builder. </exception>
        ///
        /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
        /// <param name="builder">      The builder. </param>
        /// <param name="configure">    (Optional) The configure. </param>
        ///
        /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;System.String&gt;, TClassType&gt;. </returns>

        public static IPropertyBuilder<string?, TClassType> UseTokenStringPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder, Action<TokenStringEditorAttribute>? configure = null)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            var attribute = new TokenStringEditorAttribute();
            if (configure is not null)
            {
                configure(attribute);
            }
            return builder.WithAttribute(attribute);
        }

        /// <summary>
        /// Use the Token Objects Property Editor <see cref="TokenObjectsEditorAttribute" />
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    builder. </exception>
        ///
        /// <typeparam name="TProperty">    The type of the t property. </typeparam>
        /// <typeparam name="TClassType">   The type of the type. </typeparam>
        /// <param name="builder">  The builder. </param>
        ///
        /// <returns>   IPropertyBuilder&lt;TProperty, TClassType&gt;. </returns>

        public static IPropertyBuilder<TProperty?, TClassType> UseTokenObjectsPropertyEditor<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder)
            where TProperty : IList
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            return builder.WithAttribute(new TokenObjectsEditorAttribute());
        }
    }
}
