using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Editors;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class PropertyBuilderExtensions
    {
        /// <summary>
        /// Uses the built-in alias for the BooleanPropertyEditor and ASPxBooleanPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<bool?, TClassType> UsingBooleanPropertyEditor<TClassType>(this IPropertyBuilder<bool?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.BooleanPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the BooleanPropertyEditor and ASPxBooleanPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<bool, TClassType> UsingBooleanPropertyEditor<TClassType>(this IPropertyBuilder<bool, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.BooleanPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the StringPropertyEditor and ASPxStringPropertyEditor 
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<string?, TClassType> UsingStringPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.StringPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the RichTextPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<string?, TClassType> UsingRichTextPropertyEditor<TClassType>(this IPropertyBuilder<string?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.RichTextPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the RichTextPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<byte[]?, TClassType> UsingRichTextPropertyEditor<TClassType>(this IPropertyBuilder<byte[]?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.RichTextPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the BytePropertyEditor and ASPxBytePropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<byte, TClassType> UsingBytePropertyEditor<TClassType>(this IPropertyBuilder<byte, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.BytePropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the BytePropertyEditor and ASPxBytePropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<byte?, TClassType> UsingBytePropertyEditor<TClassType>(this IPropertyBuilder<byte?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.BytePropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the DecimalPropertyEditor and ASPxDecimalPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<decimal, TClassType> UsingDecimalPropertyEditor<TClassType>(this IPropertyBuilder<decimal, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.DecimalPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the DecimalPropertyEditor and ASPxDecimalPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<decimal?, TClassType> UsingDecimalPropertyEditor<TClassType>(this IPropertyBuilder<decimal?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.DecimalPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the DoublePropertyEditor and ASPxDoublePropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<double, TClassType> UsingDoublePropertyEditor<TClassType>(this IPropertyBuilder<double, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.DoublePropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the DoublePropertyEditor and ASPxDoublePropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<double?, TClassType> UsingDoublePropertyEditor<TClassType>(this IPropertyBuilder<double?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.DoublePropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the FloatPropertyEditor and ASPxFloatPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<float, TClassType> UsingFloatPropertyEditor<TClassType>(this IPropertyBuilder<float, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.FloatPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the FloatPropertyEditor and ASPxFloatPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<float?, TClassType> UsingFloatPropertyEditor<TClassType>(this IPropertyBuilder<float?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.FloatPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<short, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<short, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<short?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<short?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<ushort, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<ushort, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<ushort?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<ushort?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<int, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<int, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<int?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<int?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<uint, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<uint, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<uint?, TClassType> UsingIntegerPropertyEditor<TClassType>(this IPropertyBuilder<uint?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<long, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<long, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the LongPropertyEditor and ASPxLongPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<long?, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<long?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the IntegerPropertyEditor and ASPxIntPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<ulong, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<ulong, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);

        /// <summary>
        /// Uses the built-in alias for the LongPropertyEditor and ASPxLongPropertyEditor
        /// </summary>
        /// <typeparam name="TClassType">The type of the type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IPropertyBuilder<ulong?, TClassType> UsingLongPropertyEditor<TClassType>(this IPropertyBuilder<ulong?, TClassType> builder)
            => builder.UsingEditorAlias(EditorAliases.IntegerPropertyEditor);
    }
}
