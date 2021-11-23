
using System;

namespace DevExpress.ExpressApp.Editors;

/// <summary>   Class WebViewPropertyEditorExtensions. </summary>
public static class WebViewPropertyEditorExtensions
{
    /// <summary>   Uses the web view URI property editor. </summary>
    ///
    /// <exception cref="ArgumentNullException">    editorDescriptorsFactory. </exception>
    ///
    /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
    ///
    /// <returns>   EditorDescriptorsFactory. </returns>

    public static EditorDescriptorsFactory UseWebViewUriPropertyEditor(this EditorDescriptorsFactory editorDescriptorsFactory)
    {
        _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

        editorDescriptorsFactory.RegisterPropertyEditorAlias(
            Xenial.Framework.WebView.PubTernal.WebViewEditorAliases.WebViewUriPropertyEditor,
            typeof(Uri),
            true
        );

        return editorDescriptorsFactory;
    }

    /// <summary>   Uses the web view HTML string property editor. </summary>
    ///
    /// <exception cref="ArgumentNullException">    editorDescriptorsFactory. </exception>
    ///
    /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>
    ///
    /// <returns>   EditorDescriptorsFactory. </returns>

    public static EditorDescriptorsFactory UseWebViewHtmlStringPropertyEditor(this EditorDescriptorsFactory editorDescriptorsFactory)
    {
        _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));

        editorDescriptorsFactory.RegisterPropertyEditorAlias(
            Xenial.Framework.WebView.PubTernal.WebViewEditorAliases.WebViewHtmlStringPropertyEditor,
            typeof(string),
            true
        );

        return editorDescriptorsFactory;
    }
}
