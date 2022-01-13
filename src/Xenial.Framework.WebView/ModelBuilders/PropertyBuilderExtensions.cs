﻿using System;

using Xenial.Framework.WebView.PubTernal;

namespace Xenial.Framework.ModelBuilders;

/// <summary>   Class PropertyBuilderExtensions. </summary>
public static class PropertyBuilderExtensions
{
    /// <summary>   Uses the web view uri property editor. </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;Uri, TClassType&gt;. </returns>

    public static IPropertyBuilder<Uri?, TClassType> UseWebViewUriPropertyEditor<TClassType>(this IPropertyBuilder<Uri?, TClassType> builder)
        => builder.UsingEditorAlias(WebViewEditorAliases.WebViewUriPropertyEditor);

    /// <summary>   Uses the web view HTML string property editor. </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;System.String&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<string?, TClassType> UseWebViewHtmlStringPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder)
        => builder.UsingEditorAlias(WebViewEditorAliases.WebViewHtmlStringPropertyEditor);
}
