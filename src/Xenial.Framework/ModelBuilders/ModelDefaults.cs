using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.ModelBuilders;

/// <summary>   Collection if ModelDefault constants. </summary>
public static class ModelDefaults
{
    /// <summary>   (Immutable) the caption. </summary>
    public const string Caption = nameof(IModelCommonMemberViewItem.Caption);

    /// <summary>   (Immutable) the is password. </summary>
    public const string IsPassword = nameof(IModelCommonMemberViewItem.IsPassword);

    /// <summary>   (Immutable) the tool tip. </summary>
    public const string ToolTip = nameof(IModelCommonMemberViewItem.ToolTip);

    /// <summary>   (Immutable) the display format. </summary>
    public const string DisplayFormat = nameof(IModelCommonMemberViewItem.DisplayFormat);

    /// <summary>   (Immutable) type of the property editor. </summary>
    public const string PropertyEditorType = nameof(IModelCommonMemberViewItem.PropertyEditorType);

    /// <summary>   (Immutable) the predefined values. </summary>
    public const string PredefinedValues = nameof(IModelCommonMemberViewItem.PredefinedValues);

    /// <summary>   (Immutable) the lookup property. </summary>
    public const string LookupProperty = nameof(IModelCommonMemberViewItem.LookupProperty);

    /// <summary>   (Immutable) the allow edit. </summary>
    public const string AllowEdit = nameof(IModelCommonMemberViewItem.AllowEdit);

    /// <summary>   (Immutable) the allow new. </summary>
    public const string AllowNew = nameof(IModelView.AllowNew);

    /// <summary>   (Immutable) the allow delete. </summary>
    public const string AllowDelete = nameof(IModelView.AllowDelete);
}
