using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
[XenialLayoutPropertyEditorItem(typeof(int), typeof(IModelPropertyEditor))]
public partial record NumberLayoutPropertyEditorItem(string ViewItemId)
    : LayoutPropertyEditorItem(ViewItemId)
{
    /// <summary>   Creates the specified property editor identifier. </summary>
    ///
    /// <param name="propertyEditorId"> The property editor identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LeafNodes.NumberLayoutPropertyEditorItem. </returns>

    public static new NumberLayoutPropertyEditorItem Create(string propertyEditorId)
        => new(propertyEditorId);
}

[XenialLayoutPropertyEditorItemMapper(typeof(NumberLayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
partial class LayoutPropertyEditorItemMapper
{
}
