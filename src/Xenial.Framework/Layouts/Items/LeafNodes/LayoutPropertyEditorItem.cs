#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1000 // Don't declare static members in generic types
#pragma warning disable IDE1006 // Naming Styles

using System;
using System.Linq.Expressions;

using DevExpress.ExpressApp.Model;

using Xenial.Data;
using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout property editor item. </summary>
[XenialCheckLicense]
public partial record LayoutPropertyEditorItem(string PropertyEditorId) : LayoutViewItem(PropertyEditorId)
{
    /// <summary>   Creates the specified property editor identifier. </summary>
    ///
    /// <param name="propertyEditorId"> The property editor identifier. </param>
    ///
    /// <returns>   Xenial.Framework.Layouts.Items.LeafNodes.LayoutPropertyEditorItem. </returns>

    public static new LayoutPropertyEditorItem Create(string propertyEditorId)
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
}

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
public partial record LayoutPropertyEditorItem<TModelClass>(string ViewItemId) : LayoutPropertyEditorItem(ViewItemId)
    where TModelClass : class
{
    /// <summary>   Gets the expression helper. </summary>
    ///
    /// <value> The expression helper. </value>

    protected static ExpressionHelper<TModelClass> ExpressionHelper { get; } = Xenial.Utils.ExpressionHelper.Create<TModelClass>();

    /// <summary>   Creates the specified expression. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <param name="expression">   The expression. </param>
    ///
    /// <returns>
    /// Xenial.Framework.Layouts.Items.LeafNodes.LayoutPropertyEditorItem&lt;TModelClass&gt;.
    /// </returns>

    public static LayoutPropertyEditorItem<TModelClass> Create<TProperty>(Expression<Func<TModelClass, TProperty>> expression)
        => new(ExpressionHelper.Property(expression));

    /// <summary>   Creates the specified expression. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <param name="expression">               The expression. </param>
    /// <param name="configurePropertyEditor">  The configure property editor. </param>
    ///
    /// <returns>
    /// Xenial.Framework.Layouts.Items.LeafNodes.LayoutPropertyEditorItem&lt;TModelClass&gt;.
    /// </returns>

    public static LayoutPropertyEditorItem<TModelClass> Create<TProperty>(Expression<Func<TModelClass, TProperty>> expression, Action<LayoutPropertyEditorItem<TModelClass>> configurePropertyEditor)
    {
        _ = configurePropertyEditor ?? throw new ArgumentNullException(nameof(configurePropertyEditor));
        var editor = new LayoutPropertyEditorItem<TModelClass>(ExpressionHelper.Property(expression));
        configurePropertyEditor(editor);
        return editor;
    }
}

/// <summary>
/// 
/// </summary>
[XenialCheckLicense]
public partial record LayoutPropertyEditorItem<TPropertyType, TModelClass>(string ViewItemId) : LayoutPropertyEditorItem<TModelClass>(ViewItemId)
    where TModelClass : class
{
    public Type PropertyType => typeof(TPropertyType);

    /// <summary>   Creates the specified expression. </summary>
    ///
    /// <param name="expression">   The expression. </param>
    ///
    /// <returns>
    /// Xenial.Framework.Layouts.Items.LeafNodes.LayoutPropertyEditorItem&lt;TModelClass&gt;.
    /// </returns>

    public static LayoutPropertyEditorItem<TPropertyType, TModelClass> Create(Expression<Func<TModelClass, TPropertyType>> expression)
        => new(ExpressionHelper.Property(expression));


    /// <summary>   Creates the specified expression. </summary>
    ///
    /// <param name="expression">               The expression. </param>
    /// <param name="configurePropertyEditor">  The configure property editor. </param>
    ///
    /// <returns>
    /// Xenial.Framework.Layouts.Items.LeafNodes.LayoutPropertyEditorItem&lt;TModelClass&gt;.
    /// </returns>

    public static LayoutPropertyEditorItem<TPropertyType, TModelClass> Create(Expression<Func<TModelClass, TPropertyType>> expression, Action<LayoutPropertyEditorItem<TModelClass>> configurePropertyEditor)
    {
        _ = configurePropertyEditor ?? throw new ArgumentNullException(nameof(configurePropertyEditor));
        var editor = new LayoutPropertyEditorItem<TPropertyType, TModelClass>(ExpressionHelper.Property(expression));
        configurePropertyEditor(editor);
        return editor;
    }
}
