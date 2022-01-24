#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1000 // Don't declare static members in generic types
#pragma warning disable IDE1006 // Naming Styles

using System;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout property editor item. </summary>
[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelPropertyEditor), IgnoredMembers = new[]
    {
        "Id",
        nameof(IModelViewItem.Caption),
        nameof(IModelPropertyEditor.Index),
    }
)]
public partial record LayoutPropertyEditorItem(string PropertyEditorId)
    : LayoutMemberViewItem(PropertyEditorId)
{
    /// <summary>   Creates the specified property editor identifier. </summary>
    ///
    /// <param name="propertyEditorId"> The property editor identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LeafNodes.LayoutPropertyEditorItem. </returns>

    public static LayoutPropertyEditorItem Create(string propertyEditorId)
        => new(propertyEditorId);

    /// <summary>   Creates the specified property editor identifier. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="propertyEditorId">         The property editor identifier. </param>
    /// <param name="configurePropertyEditor">  The configure property editor. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LeafNodes.LayoutPropertyEditorItem. </returns>

    public static LayoutPropertyEditorItem Create(string propertyEditorId, Action<LayoutPropertyEditorItem> configurePropertyEditor)
    {
        _ = configurePropertyEditor ?? throw new ArgumentNullException(nameof(configurePropertyEditor));
        var editor = new LayoutPropertyEditorItem(propertyEditorId);
        configurePropertyEditor(editor);
        return editor;
    }

    /// <summary>   Gets or sets the property editor options. </summary>
    ///
    /// <value> The property editor options. </value>

    public Action<IModelPropertyEditor>? PropertyEditorOptions { get; set; }


    private string? editorAlias;
    internal bool WasEditorAliasSet { get; set; }
    /// <summary>
    /// Specifies an editor alias.
    /// </summary>
    public string? EditorAlias
    {
        get => editorAlias;
        set
        {
            WasEditorAliasSet = true;
            editorAlias = value;
        }
    }
}
