using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.LabelEditors.Layout;

namespace Xenial.Framework.Layouts;

/// <summary>
/// 
/// </summary>
public static class LabelEditorFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlTemplate"></param>
    /// <returns></returns>
    public static HtmlContentLayoutViewItem HtmlContentLayoutViewItem(
        string htmlTemplate
    ) => new HtmlContentLayoutViewItem(htmlTemplate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlTemplate"></param>
    /// <param name="cssStyles"></param>
    /// <returns></returns>
    public static HtmlContentLayoutViewItem HtmlContentLayoutViewItem(
        string htmlTemplate, string cssStyles
    ) => new HtmlContentLayoutViewItem(htmlTemplate, cssStyles);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlTemplate"></param>
    /// <param name="cssStyles"></param>
    /// <param name="imageNames"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "<Pending>")]
    public static HtmlContentLayoutViewItem HtmlContentLayoutViewItem(
        string htmlTemplate, string cssStyles, List<string> imageNames
    ) => new HtmlContentLayoutViewItem(htmlTemplate, cssStyles, imageNames);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="htmlTemplate"></param>
    /// <param name="cssStyles"></param>
    /// <param name="imageNames"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "<Pending>")]
    public static HtmlContentLayoutViewItem HtmlContentLayoutViewItem(
        string id, string htmlTemplate, string cssStyles, List<string> imageNames
    ) => new HtmlContentLayoutViewItem(id, htmlTemplate, cssStyles, imageNames);
}

