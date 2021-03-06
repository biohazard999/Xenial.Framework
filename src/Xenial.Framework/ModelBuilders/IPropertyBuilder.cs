﻿using System;

using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>   Interface IPropertyBuilder. </summary>
    public interface IPropertyBuilder
    {
        /// <summary>   Gets the name of the property. </summary>
        ///
        /// <value> The name of the property. </value>

        string PropertyName { get; }

        /// <summary>   Gets the member information. </summary>
        ///
        /// <value> The member information. </value>

        IMemberInfo MemberInfo { get; }
    }

    /// <summary>   Interface for property builder. </summary>
    ///
    /// <typeparam name="TPropertyType">    . </typeparam>
    /// <typeparam name="TClassType">       The type of the type. </typeparam>

    public interface IPropertyBuilder<TPropertyType, TClassType> : IPropertyBuilder
    {
        /// <summary>   Removes the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        ///
        /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

        IPropertyBuilder<TPropertyType, TClassType> RemoveAttribute<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>   Removes the attribute. </summary>
        ///
        /// <param name="attribute">    The attribute. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

        IPropertyBuilder<TPropertyType, TClassType> RemoveAttribute(Attribute attribute);

        /// <summary>   Withes the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        ///
        /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

        IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>()
            where TAttribute : Attribute, new();

        /// <summary>   Withes the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        /// <param name="attribute">    The attribute. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

        IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>(TAttribute attribute)
            where TAttribute : Attribute;

        /// <summary>   Withes the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        /// <param name="configureAction">  The configure action. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

        IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>(Action<TAttribute> configureAction)
            where TAttribute : Attribute, new();

        /// <summary>   Withes the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        /// <param name="attribute">        The attribute. </param>
        /// <param name="configureAction">  (Optional) The configure action. </param>
        ///
        /// <returns>   An IPropertyBuilder&lt;TPropertyType,TClassType&gt; </returns>

        IPropertyBuilder<TPropertyType, TClassType> WithAttribute<TAttribute>(TAttribute attribute, Action<TAttribute>? configureAction = null)
            where TAttribute : Attribute;
    }
}
