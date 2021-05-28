
using System;

namespace DevExpress.ExpressApp.Editors
{
    /// <summary>   Class StepProgressEnumPropertyEditorExtensions. </summary>
    public static class StepProgressEnumPropertyEditorExtensions
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

        public static EditorDescriptorsFactory UseStepProgressEnumPropertyEditors(this EditorDescriptorsFactory editorDescriptorsFactory)
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditorAlias(
                Xenial.Framework.StepProgressEditors.PubTernal.StepProgressEditorAliases.StepProgressEnumPropertyEditor,
                typeof(Enum),
                true
            );

            return editorDescriptorsFactory;
        }

        /// <summary>   Uses the token objects property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    editorDescriptorsFactory. </exception>
        ///
        /// <typeparam name="TPropertyType">    The type of the t property type. </typeparam>
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
        ///
        /// <returns>   EditorDescriptorsFactory. </returns>
        ///
        /// ### <exception cref="System.ArgumentNullException"> editorDescriptorsFactory. </exception>

        public static EditorDescriptorsFactory UseStepProgressEnumPropertyEditor<TPropertyType>(this EditorDescriptorsFactory editorDescriptorsFactory)
            where TPropertyType : Enum
        {
            _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

            editorDescriptorsFactory.RegisterPropertyEditorAlias(
                Xenial.Framework.StepProgressEditors.PubTernal.StepProgressEditorAliases.StepProgressEnumPropertyEditor,
                typeof(TPropertyType),
                true
            );

            return editorDescriptorsFactory;
        }
    }
}
