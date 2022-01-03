using DevExpress.ExpressApp.Model;

using Xenial;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
[XenialLayoutPropertyEditorItem(typeof(bool), typeof(IModelPropertyEditor))]
public partial record BooleanLayoutPropertyEditorItem(string ViewItemId)
    : LayoutPropertyEditorItem(ViewItemId)
{
    /// <summary>   Creates the specified property editor identifier. </summary>
    ///
    /// <param name="propertyEditorId"> The property editor identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LeafNodes.StringLayoutPropertyEditorItem. </returns>

    public static new BooleanLayoutPropertyEditorItem Create(string propertyEditorId)
        => new(propertyEditorId);
}

[XenialLayoutPropertyEditorItemMapper(typeof(BooleanLayoutPropertyEditorItem), typeof(IModelPropertyEditor))]
partial class LayoutPropertyEditorItemMapper
{
}
