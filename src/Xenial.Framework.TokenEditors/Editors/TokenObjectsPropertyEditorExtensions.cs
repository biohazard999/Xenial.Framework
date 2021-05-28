
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DevExpress.ExpressApp.Editors
{
    /// <summary>   Class TokenObjectsPropertyEditorExtensions. </summary>
    public static class TokenObjectsPropertyEditorExtensions
    {
        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>
        ///
        /// ### <exception cref="System.ArgumentNullException"> editorDescriptorsFactory. </exception>

        public static EditorDescriptorsFactory UseTokenObjectsPropertyEditors(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditorAlias(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(IList<>),
                true
            );

            return editorDescriptorsFactory;
        }

        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="T">    . </typeparam>
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>
        ///
        /// ### <exception cref="System.ArgumentNullException"> editorDescriptorsFactory. </exception>

        public static EditorDescriptorsFactory UseTokenObjectsPropertyEditors<T>(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditorAlias(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(IList<T>),
                true
            );

            editorDescriptorsFactory.RegisterPropertyEditorAlias(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(BindingList<T>),
                true
            );

            return editorDescriptorsFactory;
        }

        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <typeparam name="T">    . </typeparam>
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>
        ///
        /// ### <exception cref="System.ArgumentNullException"> editorDescriptorsFactory. </exception>

        public static EditorDescriptorsFactory UseTokenObjectsPropertyEditorsForType<T>(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditorAlias(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenObjectsPropertyEditor,
                typeof(T),
                true
            );

            return editorDescriptorsFactory;
        }
    }
}
