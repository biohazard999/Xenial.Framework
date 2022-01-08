using System;

using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.ModelBuilders;

public static partial class PropertyBuilderExtensions
{
    /// <summary>   Determines whether the specified caption has caption. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    /// <param name="caption">  The caption. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasCaption<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string caption)
        => builder.WithModelDefault(ModelDefaults.Caption, caption);

    /// <summary>   Determines whether the specified tooltip has tooltip. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    /// <param name="nullText"> The tooltip. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasNullText<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string nullText)
        => builder.WithModelDefault("NullText", nullText);

    /// <summary>   Determines whether the specified tooltip has tooltip. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    /// <param name="tooltip">  The tooltip. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasTooltip<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string tooltip)
        => builder.WithModelDefault(ModelDefaults.ToolTip, tooltip);

    /// <summary>   Determines whether the specified tooltip has tooltip. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">      The builder. </param>
    /// <param name="tooltipTitle"> The tooltip. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasTooltipTitle<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string tooltipTitle)
        => builder.WithModelDefault("ToolTipTitle", tooltipTitle);

    /// <summary>   Determines whether the specified tooltip has tooltip. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">          The builder. </param>
    /// <param name="toolTipIconType">  The tooltip. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasTooltipIconType<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, ToolTipIconType toolTipIconType)
        => builder.WithModelDefault("ToolTipIconType", toolTipIconType.ToString());

    /// <summary>   Determines whether [has display format] [the specified display format]. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">          The builder. </param>
    /// <param name="displayFormat">    The display format. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasDisplayFormat<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string displayFormat)
        => builder.WithModelDefault(ModelDefaults.DisplayFormat, displayFormat);

    /// <summary>   Determines whether [has edit mask] [the specified edit mask]. </summary>
    ///
    /// <typeparam name="TProperty">    The type of the t property. </typeparam>
    /// <typeparam name="TClassType">   The type of the t class type. </typeparam>
    /// <param name="builder">      The builder. </param>
    /// <param name="editMask">     The edit mask. </param>
    /// <param name="editMaskType"> Type of the edit mask. </param>
    ///
    /// <returns>   IPropertyBuilder&lt;System.Nullable&lt;TProperty&gt;, TClassType&gt;. </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasEditMask<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, string editMask, EditMaskType? editMaskType = null)
    {
        builder.WithModelDefault("EditMask", editMask);
        if (editMaskType.HasValue)
        {
            builder.WithModelDefault("EditMaskType", editMaskType.Value.ToString());
        }
        return builder;
    }

    /// <summary>   Determines whether the specified index has index. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="TProperty">    The type of the property. </typeparam>
    /// <typeparam name="TClassType">   The type of the type. </typeparam>
    /// <param name="builder">  The builder. </param>
    /// <param name="index">    The index. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TProperty?,TClassType&gt; </returns>

    public static IPropertyBuilder<TProperty?, TClassType> HasIndex<TProperty, TClassType>(this IPropertyBuilder<TProperty?, TClassType> builder, int index)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        return builder.WithAttribute(new IndexAttribute(index));
    }
}
