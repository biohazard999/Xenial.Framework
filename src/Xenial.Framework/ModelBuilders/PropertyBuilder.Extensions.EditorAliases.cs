using System;
using System.ComponentModel;
using System.Drawing;

using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders;

public static partial class PropertyBuilderExtensions
{
    /// <summary>
    /// Uses the built-in alias for the BooleanPropertyEditor and ASPxBooleanPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;bool?,TClassType&gt; </returns>

    public static IPropertyBuilder<bool?, TClassType> UsingBooleanPropertyEditor<TClassType>(this IPropertyBuilder<bool?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.BooleanPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the BooleanPropertyEditor and ASPxBooleanPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;bool,TClassType&gt; </returns>

    public static IPropertyBuilder<bool, TClassType> UsingBooleanPropertyEditor<TClassType>(this IPropertyBuilder<bool, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.BooleanPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the StringPropertyEditor and ASPxStringPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;string?,TClassType&gt; </returns>

    public static IPropertyBuilder<string?, TClassType> UsingStringPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.StringPropertyEditor);

    /// <summary>   Uses the built-in alias for the RichTextPropertyEditor. </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;string?,TClassType&gt; </returns>

    public static IPropertyBuilder<string?, TClassType> UsingRichTextPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.RichTextPropertyEditor);

    /// <summary>   Uses the built-in alias for the RichTextPropertyEditor. </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;byte[]?,TClassType&gt; </returns>

    public static IPropertyBuilder<byte[]?, TClassType> UsingRichTextPropertyEditor<TClassType>(this IPropertyBuilder<byte[]?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.RichTextPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the BytePropertyEditor and ASPxBytePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;byte,TClassType&gt; </returns>

    public static IPropertyBuilder<byte, TClassType> UsingBytePropertyEditor<TClassType>(this IPropertyBuilder<byte, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.BytePropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the BytePropertyEditor and ASPxBytePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;byte?,TClassType&gt; </returns>

    public static IPropertyBuilder<byte?, TClassType> UsingBytePropertyEditor<TClassType>(this IPropertyBuilder<byte?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.BytePropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the DecimalPropertyEditor and ASPxDecimalPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;decimal,TClassType&gt; </returns>

    public static IPropertyBuilder<decimal, TClassType> UsingDecimalPropertyEditor<TClassType>(this IPropertyBuilder<decimal, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DecimalPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the DecimalPropertyEditor and ASPxDecimalPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;decimal?,TClassType&gt; </returns>

    public static IPropertyBuilder<decimal?, TClassType> UsingDecimalPropertyEditor<TClassType>(this IPropertyBuilder<decimal?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DecimalPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the DoublePropertyEditor and ASPxDoublePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;double,TClassType&gt; </returns>

    public static IPropertyBuilder<double, TClassType> UsingDoublePropertyEditor<TClassType>(this IPropertyBuilder<double, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DoublePropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the DoublePropertyEditor and ASPxDoublePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;double?,TClassType&gt; </returns>

    public static IPropertyBuilder<double?, TClassType> UsingDoublePropertyEditor<TClassType>(this IPropertyBuilder<double?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DoublePropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the FloatPropertyEditor and ASPxFloatPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;float,TClassType&gt; </returns>

    public static IPropertyBuilder<float, TClassType> UsingFloatPropertyEditor<TClassType>(this IPropertyBuilder<float, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.FloatPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the FloatPropertyEditor and ASPxFloatPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;float?,TClassType&gt; </returns>

    public static IPropertyBuilder<float?, TClassType> UsingFloatPropertyEditor<TClassType>(this IPropertyBuilder<float?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.FloatPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;short,TClassType&gt; </returns>

    public static IPropertyBuilder<short, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<short, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;short?,TClassType&gt; </returns>

    public static IPropertyBuilder<short?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<short?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;ushort,TClassType&gt; </returns>

    public static IPropertyBuilder<ushort, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<ushort, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;ushort?,TClassType&gt; </returns>

    public static IPropertyBuilder<ushort?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<ushort?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;int,TClassType&gt; </returns>

    public static IPropertyBuilder<int, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<int, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;int?,TClassType&gt; </returns>

    public static IPropertyBuilder<int?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<int?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;uint,TClassType&gt; </returns>

    public static IPropertyBuilder<uint, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<uint, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;uint?,TClassType&gt; </returns>

    public static IPropertyBuilder<uint?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<uint?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;long,TClassType&gt; </returns>

    public static IPropertyBuilder<long, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<long, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the LongPropertyEditor and ASPxLongPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;long?,TClassType&gt; </returns>

    public static IPropertyBuilder<long?, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<long?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;ulong,TClassType&gt; </returns>

    public static IPropertyBuilder<ulong, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<ulong, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the LongPropertyEditor and ASPxLongPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;ulong?,TClassType&gt; </returns>

    public static IPropertyBuilder<ulong?, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<ulong?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the DatePropertyEditor and ASPxDateTimePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;DateTime&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<DateTime, TClassType> UsingDateTimePropertyEditor<TClassType>(this IPropertyBuilder<DateTime, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DateTimePropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the DatePropertyEditor and ASPxDateTimePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;DateTime&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<DateTime?, TClassType> UsingDateTimePropertyEditor<TClassType>(this IPropertyBuilder<DateTime?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DateTimePropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the TimeSpanPropertyEditor and ASPxTimeSpanPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TimeSpan&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<TimeSpan, TClassType> UsingTimeSpanPropertyEditor<TClassType>(this IPropertyBuilder<TimeSpan, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.TimeSpanPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the TimeSpanPropertyEditor and ASPxTimeSpanPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TimeSpan&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<TimeSpan?, TClassType> UsingTimeSpanPropertyEditor<TClassType>(this IPropertyBuilder<TimeSpan?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.TimeSpanPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the ImagePropertyEditor and ASPxImagePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    /// <param name="options">  (Optional) The options. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Byte[], TClassType&gt;. </returns>

    public static IPropertyBuilder<byte[]?, TClassType> UsingImagePropertyEditor<TClassType>(this IPropertyBuilder<byte[]?, TClassType> builder, Action<ImageEditorAttribute>? options = null)
        => builder
            .UsingEditorAlias(EditorAliases.ImagePropertyEditor)
            .WithAttribute(new ImageEditorAttribute(), options)
        ;

    /// <summary>   Uses the built-in alias for the DetailPropertyEditor. </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TimeSpan&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<object?, TClassType> UsingDetailPropertyEditor<TClassType>(this IPropertyBuilder<object?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DetailPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the LookupPropertyEditor and ASPxLookupPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TimeSpan&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<object?, TClassType> UsingLookupPropertyEditor<TClassType>(this IPropertyBuilder<object?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.LookupPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the ObjectPropertyEditor and ASPxObjectPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TimeSpan&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<object?, TClassType> UsingObjectPropertyEditor<TClassType>(this IPropertyBuilder<object?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.ObjectPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the EnumPropertyEditor and ASPxEnumPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TEnum">        The type of the t enum. </typeparam>
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;TEnum, TClassType&gt;. </returns>

    public static IPropertyBuilder<TEnum?, TClassType> UsingEnumPropertyEditor<TEnum, TClassType>(this IPropertyBuilder<TEnum?, TClassType> builder)
        where TEnum : struct, Enum
            => builder.UsingEditorAlias(EditorAliases.EnumPropertyEditor);

    /// <summary>   Uses the built-in alias for the Type Properties PropertyEditor. </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<Type?, TClassType> UsingTypePropertyEditor<TClassType>(this IPropertyBuilder<Type?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.TypePropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the VisibleInReportsTypePropertyEditor and
    /// ASPxVisibleInReportsTypePropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<Type?, TClassType> UsingVisibleInReportsTypePropertyEditor<TClassType>(this IPropertyBuilder<Type?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.VisibleInReportsTypePropertyEditor);

    /// <summary>
    /// Uses Specifies the built-in alias for the CriteriaPropertyEditor,
    /// AdvancedCriteriaPropertyEditor,
    ///     and ASPxCriteriaPropertyEditor editors. The static
    ///     DevExpress.XtraEditors.WindowsFormsSettings.UseAdvancedFilterEditorControl property
    ///     determines which editor a WinForms application uses (DevExpress.Utils.DefaultBoolean.False
    ///     - for CriteriaPropertyEditor)
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">              The builder. </param>
    /// <param name="objectTypeMemberName"> Name of the object type member. </param>
    /// <param name="parametersMemberName"> (Optional) Name of the parameters member. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<string?, TClassType> UsingCriteriaPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder, string objectTypeMemberName, string? parametersMemberName = null)
        => builder
            .UsingEditorAlias(EditorAliases.CriteriaPropertyEditor)
            .WithAttribute(new CriteriaOptionsAttribute(objectTypeMemberName, parametersMemberName))
        ;

    /// <summary>   Uses the built-in alias for the ExtendedCriteria Property Editor. </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">              The builder. </param>
    /// <param name="objectTypeMemberName"> Name of the object type member. </param>
    /// <param name="parametersMemberName"> (Optional) Name of the parameters member. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<string?, TClassType> UsingExtendedCriteriaPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder, string objectTypeMemberName, string? parametersMemberName = null)
        => builder
            .UsingEditorAlias(EditorAliases.ExtendedCriteriaPropertyEditor)
            .WithAttribute(new CriteriaOptionsAttribute(objectTypeMemberName, parametersMemberName))
        ;

    /// <summary>
    /// Uses the built-in alias for the PopupCriteriaPropertyEditor and
    /// ASPxPopupCriteriaPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">              The builder. </param>
    /// <param name="objectTypeMemberName"> Name of the object type member. </param>
    /// <param name="parametersMemberName"> (Optional) Name of the parameters member. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<string?, TClassType> UsingPopupCriteriaPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder, string objectTypeMemberName, string? parametersMemberName = null)
        => builder
            .UsingEditorAlias(EditorAliases.PopupCriteriaPropertyEditor)
            .WithAttribute(new CriteriaOptionsAttribute(objectTypeMemberName, parametersMemberName))
        ;

    /// <summary>
    /// Uses the built-in alias for the ColorPropertyEditor and ASPxColorPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<Color, TClassType> UsingColorPropertyEditor<TClassType>(this IPropertyBuilder<Color, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.ColorPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the ColorPropertyEditor and ASPxColorPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<Color?, TClassType> UsingColorPropertyEditor<TClassType>(this IPropertyBuilder<Color?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.ColorPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the DefaultPropertyEditor and ASPxDefaultPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IPropertyBuilder<object?, TClassType> UsingDefaultPropertyEditor<TClassType>(this IPropertyBuilder<object?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.DefaultPropertyEditor);

    /// <summary>
    /// Uses the built-in alias for the ProtectedContentPropertyEditor and
    /// ASPxProtectedContentPropertyEditor.
    /// </summary>
    ///
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">  The builder. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;Type&gt;, TClassType&gt;. </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IPropertyBuilder<object?, TClassType> UsingProtectedContentPropertyEditor<TClassType>(this IPropertyBuilder<object?, TClassType> builder)
        => builder.UsingEditorAlias(EditorAliases.ProtectedContentPropertyEditor);
}
