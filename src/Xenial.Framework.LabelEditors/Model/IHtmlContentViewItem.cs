
using DevExpress.ExpressApp.Model;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using System.Linq;

namespace Xenial.Framework.LabelEditors.Model;

/// <summary>
/// 
/// </summary>
public interface IHtmlContentViewItem : IModelViewItem
{
    /// <summary>
    /// 
    /// </summary>
    [Editor("Xenial.Framework.LabelEditors.Win.Model.Core.XenialHtmlTemplateUITypeEditor, Xenial.Framework.LabelEditors.Win", DevExpress.Utils.ControlConstants.UITypeEditor)]
    string HtmlTemplate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Editor("Xenial.Framework.LabelEditors.Win.Model.Core.XenialCssTemplateUITypeEditor, Xenial.Framework.LabelEditors.Win", DevExpress.Utils.ControlConstants.UITypeEditor)]
    string CssStyles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DefaultValue(false)]
    [RefreshProperties(RefreshProperties.All)]
    bool LoadAllImages { get; set; }

    /// <summary>
    /// 
    /// </summary>
    //TODO: Image Editor
    [ModelBrowsable(typeof(ImageNamesEditorVisibilityCalculator))]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "By Design")]
    string[] ImageNames { get; set; }
}

/// <summary>
/// 
/// </summary>
[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ImageNamesEditorVisibilityCalculator : IModelIsVisible
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public bool IsVisible(IModelNode node!!, string propertyName)
    {
        var visible = !node.GetValue<bool>(nameof(IHtmlContentViewItem.LoadAllImages));
        return visible;
    }
}

