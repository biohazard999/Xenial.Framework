using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
[XenialLayoutPropertyEditorItem(typeof(string), typeof(IModelPropertyEditor))]
public partial record StringLayoutPropertyEditorItem(string ViewItemId)
    : LayoutPropertyEditorItem(ViewItemId)
{
    /// <summary>   Creates the specified property editor identifier. </summary>
    ///
    /// <param name="propertyEditorId"> The property editor identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LeafNodes.StringLayoutPropertyEditorItem. </returns>
    public static new StringLayoutPropertyEditorItem Create(string propertyEditorId)
        => new(propertyEditorId);
}

[XenialLayoutPropertyEditorItemMapper(typeof(StringLayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
partial class LayoutPropertyEditorItemMapper
{
}
