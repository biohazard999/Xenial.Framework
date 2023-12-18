using System;
using System.ComponentModel;

using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders;

/// <summary>   A property builder. </summary>
public static class PropertyBuilder
{
    /// <summary>   Properties the builder for. </summary>
    ///
    /// <typeparam name="TPropertyType">    The type of the property type. </typeparam>
    /// <typeparam name="TClassType">       The type of the class type. </typeparam>
    /// <param name="member">   The member. </param>
    ///
    /// <returns>   A PropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

    public static PropertyBuilder<TPropertyType, TClassType> PropertyBuilderFor<TPropertyType, TClassType>(IMemberInfo member)
        => new(member);
}

/// <summary>   A property builder. </summary>
///
/// <typeparam name="TPropertyType">    The type of the property. </typeparam>
/// <typeparam name="TClassType">       The type of the class. </typeparam>
///
/// <seealso cref="BuilderManager"/>
/// <seealso cref="IPropertyBuilder{TPropertyType,TClassType}"/>

public class PropertyBuilder<TPropertyType, TClassType> : BuilderManager, IPropertyBuilder<TPropertyType, TClassType>
{
    /// <summary>   Gets the member information. </summary>
    ///
    /// <value> The member information. </value>

    public IMemberInfo MemberInfo { get; }

    /// <summary>   Gets the name of the property. </summary>
    ///
    /// <value> The name of the property. </value>

    public string PropertyName => MemberInfo.Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBuilder{TProperty, TClass}"/> class.
    /// </summary>
    ///
    /// <param name="memberInfo">   The member information. </param>

    public PropertyBuilder(IMemberInfo memberInfo)
        => MemberInfo = memberInfo;

    /// <summary>   Withes the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
    /// <param name="attribute">        The attribute. </param>
    /// <param name="configureAction">  (Optional) The configure action. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

    public IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>(TAttribute attribute, Action<TAttribute>? configureAction = null)
        where TAttribute : Attribute
    {
        configureAction?.Invoke(attribute);
        MemberInfo.AddAttribute(attribute);

        return this;
    }

    /// <summary>   Withes the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
    /// <param name="configureAction">  (Optional) The configure action. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

    public IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>(Action<TAttribute>? configureAction = null)
        where TAttribute : Attribute, new()
            => WithAttribute(new TAttribute(), configureAction);

    /// <summary>   Removes the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
    ///
    /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IPropertyBuilder<TPropertyType, TClassType> RemoveAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var att = MemberInfo.FindAttribute<TAttribute>();

        return RemoveAttribute(att);
    }

    /// <summary>   Removes the attribute. </summary>
    ///
    /// <param name="attribute">    The attribute. </param>
    ///
    /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IPropertyBuilder<TPropertyType, TClassType> RemoveAttribute(Attribute attribute)
    {
        if (attribute != null && MemberInfo is XafMemberInfo xafMemberInfo)
        {
            xafMemberInfo.RemoveAttribute(attribute);
        }

        return this;
    }

    /// <summary>   Withes the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the t attribute. </typeparam>
    ///
    /// <returns>   Xenial.Framework.ModelBuilders.IPropertyBuilder&lt;TProperty, TClass&gt;. </returns>

    public IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>()
        where TAttribute : Attribute, new()
            => WithAttribute(new TAttribute(), null);

    /// <summary>   Withes the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the t attribute. </typeparam>
    /// <param name="attribute">    The attribute. </param>
    ///
    /// <returns>   Xenial.Framework.ModelBuilders.IPropertyBuilder&lt;TProperty, TClass&gt;. </returns>

    public IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>(TAttribute attribute)
        where TAttribute : Attribute
            => WithAttribute(attribute, null);
}
