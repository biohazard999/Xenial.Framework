using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
//[XenialLayoutPropertyEditorItem(typeof(object), typeof(IModelPropertyEditor))]
public partial record LookupLayoutPropertyEditorItem(string ViewItemId)
    : LayoutPropertyEditorItem(ViewItemId)
{
    /// <summary>   Creates the specified property editor identifier. </summary>
    ///
    /// <param name="propertyEditorId"> The property editor identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LeafNodes.StringLayoutPropertyEditorItem. </returns>
    public static new LookupLayoutPropertyEditorItem Create(string propertyEditorId)
        => new(propertyEditorId);
}
