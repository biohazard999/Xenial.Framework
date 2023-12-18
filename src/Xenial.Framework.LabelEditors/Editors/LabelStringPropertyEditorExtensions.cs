
using System;

namespace DevExpress.ExpressApp.Editors;

/// <summary>   Class LabelStringPropertyEditorExtensions. </summary>
public static partial class LabelStringPropertyEditorExtensions
{
    /// <summary>   Uses the label string property editor. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
    ///
    /// <returns>   EditorDescriptorsFactory. </returns>

    public static EditorDescriptorsFactory UseLabelStringPropertyEditors(this EditorDescriptorsFactory editorDescriptorsFactory)
    {
        _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

        editorDescriptorsFactory.RegisterPropertyEditorAlias(
            Xenial.Framework.LabelEditors.PubTernal.LabelEditorAliases.LabelStringPropertyEditor,
            typeof(string),
            true
        );

        return editorDescriptorsFactory;
    }
}
