using System;

namespace DevExpress.ExpressApp.Editors
{
    /// <summary>   Class TokenStringPropertyEditorExtensions. </summary>
    public static class TokenStringPropertyEditorExtensions
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

        public static EditorDescriptorsFactory UseTokenStringPropertyEditors(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditorAlias(
                Xenial.Framework.TokenEditors.PubTernal.TokenEditorAliases.TokenStringPropertyEditor,
                typeof(string),
                true
            );

            return editorDescriptorsFactory;
        }
    }
}
