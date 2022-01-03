using DevExpress.ExpressApp.Model;

using Xenial;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
[XenialLayoutPropertyEditorItem(typeof(bool), typeof(IModelPropertyEditor))]
public partial record BooleanLayoutPropertyEditorItem<TModelClass>(string ViewItemId)
    : LayoutPropertyEditorItem<bool, TModelClass>(ViewItemId)
    where TModelClass : class
{
}

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
[XenialLayoutPropertyEditorItem(typeof(string), typeof(IModelPropertyEditor))]
public partial record StringLayoutPropertyEditorItem<TModelClass>(string ViewItemId)
    : LayoutPropertyEditorItem<string, TModelClass>(ViewItemId)
    where TModelClass : class
{
}

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
[XenialLayoutPropertyEditorItem(typeof(int), typeof(IModelPropertyEditor))]
public partial record IntegerLayoutPropertyEditorItem<TModelClass>(string ViewItemId)
    : LayoutPropertyEditorItem<int, TModelClass>(ViewItemId)
    where TModelClass : class
{
}
