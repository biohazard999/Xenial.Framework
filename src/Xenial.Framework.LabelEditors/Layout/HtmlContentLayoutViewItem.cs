using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Utils;

namespace Xenial.Framework.LabelEditors.Layout;

/// <summary>
/// 
/// </summary>
public partial record HtmlContentLayoutViewItem : LayoutViewItem
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlTemplate"></param>
    public HtmlContentLayoutViewItem(string htmlTemplate)
        : this(htmlTemplate, htmlTemplate, "", new()) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlTemplate"></param>
    /// <param name="cssStyles"></param>
    public HtmlContentLayoutViewItem(string htmlTemplate, string cssStyles)
        : this(htmlTemplate, htmlTemplate, cssStyles, new()) { }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlTemplate"></param>
    /// <param name="cssStyles"></param>
    /// <param name="imageNames"></param>
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "By Design")]
    public HtmlContentLayoutViewItem(string htmlTemplate, string cssStyles, List<string> imageNames)
        : this(htmlTemplate, htmlTemplate, cssStyles, imageNames) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="htmlTemplate"></param>
    /// <param name="cssStyles"></param>
    /// <param name="imageNames"></param>
    public HtmlContentLayoutViewItem(string id, string htmlTemplate, string cssStyles, List<string> imageNames)
        : base(id)
        => (HtmlTemplate, CssStyles, ImageNames) = (htmlTemplate, cssStyles, imageNames);

    /// <summary>
    /// 
    /// </summary>
    public string HtmlTemplate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CssStyles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool LoadAllImages { get; set; }

    private List<string> imageNames = new();


    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "By Design")]
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "By Design")]
    public List<string> ImageNames
    {
        get => imageNames; set
        {
            imageNames = value;

            LoadAllImages = imageNames is not null && imageNames.Count > 0
                ? false
                : true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="htmlTemplateResourceName"></param>
    /// <param name="cssStylesResourceName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static HtmlContentLayoutViewItem FromResource(Type type, string? htmlTemplateResourceName = null, string? cssStylesResourceName = null)
    {
        _ = type ?? throw new ArgumentNullException(nameof(type));

        static bool TryGetResource(Type type, string? resourceName,
#if NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
        out string? resource)
        {
            if (resourceName is null)
            {
                resource = null;
                return false;
            }
            resource = ResourceUtil.GetResourceString(type, resourceName);
            return true;
        }

        var htmlTemplate = TryGetResource(type, htmlTemplateResourceName, out var html)
            ? html : "";

        var cssTemplate = TryGetResource(type, cssStylesResourceName, out var css)
            ? css : "";

        return new HtmlContentLayoutViewItem(htmlTemplate, cssTemplate);
    }
}

