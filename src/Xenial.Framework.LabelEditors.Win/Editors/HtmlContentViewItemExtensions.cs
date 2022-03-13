
using System;

using Xenial.Framework.LabelEditors.Model;
using Xenial.Framework.LabelEditors.Win.Editors;

namespace DevExpress.ExpressApp.Editors;

/// <summary>
/// 
/// </summary>
public static class HtmlContentViewItemExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="editorDescriptorsFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static EditorDescriptorsFactory UseXenialHtmlContentViewItemWin(this EditorDescriptorsFactory editorDescriptorsFactory)
    {
        _ = editorDescriptorsFactory ?? throw new ArgumentNullException(nameof(editorDescriptorsFactory));
        editorDescriptorsFactory.RegisterViewItem(typeof(IHtmlContentViewItem), typeof(HtmlContentWindowsFormsViewItem), true);
        return editorDescriptorsFactory;
    }
}
