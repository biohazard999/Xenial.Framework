﻿using System;
using System.Collections;
using System.Linq.Expressions;

using DevExpress.ExpressApp.DC;

using Xenial.Data;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>   Interface IModelBuilder. </summary>
    public interface IModelBuilder : IBuilder
    {
        /// <summary>   Gets the type information. </summary>
        ///
        /// <value> The type information. </value>

        ITypeInfo TypeInfo { get; }

        /// <summary>   Gets the type of the target. </summary>
        ///
        /// <value> The type of the target. </value>

        Type TargetType { get; }
    }

    /// <summary>   Interface for model builder. </summary>
    ///
    /// <typeparam name="TClassType">   . </typeparam>

    public interface IModelBuilder<TClassType> : IModelBuilder
    {
        /// <summary>   Configures the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        /// <param name="action">       The action. </param>
        /// <param name="predicate">    (Optional) The predicate. </param>
        ///
        /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

        IModelBuilder<TClassType> ConfigureAttribute<TAttribute>(Action<TAttribute> action, Func<TAttribute, bool>? predicate = null)
            where TAttribute : Attribute;

        /// <summary>   Finds the attribute. </summary>
        ///
        /// <typeparam name="TAttr">    The type of the attribute. </typeparam>
        /// <param name="predicate">    (Optional) The predicate. </param>
        ///
        /// <returns>   The found attribute. </returns>

        TAttr? FindAttribute<TAttr>(Func<TAttr, bool>? predicate = null)
            where TAttr : Attribute;

        /// <summary>   Fors the specified property. </summary>
        ///
        /// <typeparam name="TPropertyType">    The type of the property. </typeparam>
        /// <param name="propertyExpression">   The property. </param>
        ///
        /// <returns>   A PropertyBuilder&lt;TPropertyType?,TClassType&gt; </returns>

        PropertyBuilder<TPropertyType?, TClassType> For<TPropertyType>(Expression<Func<TClassType, TPropertyType?>> propertyExpression);

        /// <summary>   Nesteds the ListView identifier. </summary>
        ///
        /// <typeparam name="TListType">    The type of the ret. </typeparam>
        /// <param name="listItemExpression">   The expression. </param>
        ///
        /// <returns>   The nested list view identifier. </returns>

        string GetNestedListViewId<TListType>(Expression<Func<TClassType, TListType>> listItemExpression)
            where TListType : IEnumerable;

        /// <summary>   Removes the attribute. </summary>
        ///
        /// <typeparam name="TAttr">    The type of the attribute. </typeparam>
        /// <param name="predicate">    (Optional) The predicate. </param>
        ///
        /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

        IModelBuilder<TClassType> RemoveAttribute<TAttr>(Func<TAttr, bool>? predicate = null)
            where TAttr : Attribute;

        /// <summary>   Removes the attribute. </summary>
        ///
        /// <param name="attributeType">    Type of the attribute. </param>
        ///
        /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

        IModelBuilder<TClassType> RemoveAttribute(Type attributeType);

        /// <summary>   Withes the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        /// <param name="configureAction">  (Optional) The configure action. </param>
        ///
        /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

        IModelBuilder<TClassType> WithAttribute<TAttribute>(Action<TAttribute>? configureAction = null)
            where TAttribute : Attribute, new();

        /// <summary>   Withes the attribute. </summary>
        ///
        /// <param name="attribute">    The attribute. </param>
        ///
        /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

        IModelBuilder<TClassType> WithAttribute(Attribute attribute);

        /// <summary>   Withes the attribute. </summary>
        ///
        /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
        /// <param name="attribute">        The attribute. </param>
        /// <param name="configureAction">  (Optional) The configure action. </param>
        ///
        /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

        IModelBuilder<TClassType> WithAttribute<TAttribute>(TAttribute attribute, Action<TAttribute>? configureAction = null)
            where TAttribute : Attribute;

        /// <summary>   Gets the default detail view. </summary>
        ///
        /// <value> The default detail view. </value>

        string DefaultDetailView { get; }

        /// <summary>   Gets the default ListView. </summary>
        ///
        /// <value> The default ListView. </value>

        string DefaultListView { get; }

        /// <summary>   Gets the default lookup ListView. </summary>
        ///
        /// <value> The default lookup ListView. </value>

        string DefaultLookupListView { get; }

        /// <summary>   Gets the exp. </summary>
        ///
        /// <value> The exp. </value>

        ExpressionHelper<TClassType> ExpressionHelper { get; }
    }
}
