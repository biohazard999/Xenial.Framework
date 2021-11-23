using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Data;

namespace Xenial.Framework.ModelBuilders;

/// <summary>   A model builder. </summary>
public static partial class ModelBuilder
{
    /// <summary>   Finds the type information. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <typeparam name="T">    . </typeparam>
    /// <param name="typesInfo">    The types information. </param>
    ///
    /// <returns>   The found type information. </returns>

    public static ITypeInfo FindTypeInfo<T>(this ITypesInfo typesInfo)
    {
        _ = typesInfo ?? throw new ArgumentNullException(nameof(typesInfo));
        return typesInfo.FindTypeInfo(typeof(T));
    }

    /// <summary>   Creates the specified types information. </summary>
    ///
    /// <typeparam name="TClassType">   . </typeparam>
    /// <param name="typesInfo">    The types information. </param>
    ///
    /// <returns>   A ModelBuilder&lt;TClassType&gt; </returns>

    public static ModelBuilder<TClassType> Create<TClassType>(ITypesInfo typesInfo)
        => new(typesInfo.FindTypeInfo<TClassType>());

    /// <summary>   Creates the specified types information. </summary>
    ///
    /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
    ///                                                 are null. </exception>
    /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
    ///                                                 invalid. </exception>
    ///
    /// <typeparam name="TBuilder"> The type of the builder. </typeparam>
    /// <typeparam name="T">        . </typeparam>
    /// <param name="typesInfo">    The types information. </param>
    ///
    /// <returns>   A TBuilder. </returns>

    public static TBuilder Create<TBuilder, T>(ITypesInfo typesInfo)
        where TBuilder : IModelBuilder<T>

    {
        _ = typesInfo ?? throw new ArgumentNullException(nameof(typesInfo));

        var instance = Activator.CreateInstance(typeof(TBuilder), typesInfo.FindTypeInfo<T>());
        if (instance is TBuilder tInstance)
        {
            return tInstance;
        }

        throw new InvalidOperationException(nameof(instance));
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TClassType"></typeparam>
[XenialCheckLicense]
public partial class ModelBuilder<TClassType> : BuilderManager, ITypeInfoProvider, IModelBuilder<TClassType>
{
    /// <summary>   Initializes a new instance of the <see cref="ModelBuilder{T}"/> class. </summary>
    ///
    /// <param name="typeInfo"> The type information. </param>

    public ModelBuilder(ITypeInfo typeInfo) => TypeInfo = typeInfo;

    /// <summary>   Gets the type information. </summary>
    ///
    /// <value> The type information. </value>

    public ITypeInfo TypeInfo { get; }

    /// <summary>   Gets the type of the target. </summary>
    ///
    /// <value> The type of the target. </value>

    public Type TargetType { get; } = typeof(TClassType);

    /// <summary>   Gets the exp. </summary>
    ///
    /// <value> The exp. </value>

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ExpressionHelper<TClassType> ExpressionHelper { get; } = Xenial.Utils.ExpressionHelper.Create<TClassType>();

    /// <summary>   Gets the default detail view. </summary>
    ///
    /// <value> The default detail view. </value>

    public virtual string DefaultDetailView => ModelNodeIdHelper.GetDetailViewId(typeof(TClassType));

    /// <summary>   Gets the default ListView. </summary>
    ///
    /// <value> The default ListView. </value>

    public virtual string DefaultListView => ModelNodeIdHelper.GetListViewId(typeof(TClassType));

    /// <summary>   Gets the default lookup ListView. </summary>
    ///
    /// <value> The default lookup ListView. </value>

    public virtual string DefaultLookupListView => ModelNodeIdHelper.GetLookupListViewId(typeof(TClassType));

    /// <summary>   Nesteds the ListView identifier. </summary>
    ///
    /// <typeparam name="TListType">    The type of the ret. </typeparam>
    /// <param name="listItemExpression">   The expr. </param>
    ///
    /// <returns>   The nested list view identifier. </returns>

    public virtual string GetNestedListViewId<TListType>(Expression<Func<TClassType, TListType>> listItemExpression)
        where TListType : IEnumerable
            => ModelNodeIdHelper.GetNestedListViewId(typeof(TClassType), ExpressionHelper.Property(listItemExpression));

    /// <summary>   Withes the attribute. </summary>
    ///
    /// <param name="attribute">    The attribute. </param>
    ///
    /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

    public IModelBuilder<TClassType> WithAttribute(Attribute attribute)
    {
        TypeInfo.AddAttribute(attribute);
        return this;
    }

    /// <summary>   Withes the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
    /// <param name="attribute">        The attribute. </param>
    /// <param name="configureAction">  (Optional) The configure action. </param>
    ///
    /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

    public IModelBuilder<TClassType> WithAttribute<TAttribute>(TAttribute attribute, Action<TAttribute>? configureAction = null)
        where TAttribute : Attribute
    {
        configureAction?.Invoke(attribute);
        return WithAttribute((Attribute)attribute);
    }

    /// <summary>   Withes the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
    /// <param name="configureAction">  (Optional) The configure action. </param>
    ///
    /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

    public IModelBuilder<TClassType> WithAttribute<TAttribute>(Action<TAttribute>? configureAction = null)
        where TAttribute : Attribute, new()
        => WithAttribute(new TAttribute(), configureAction);

    /// <summary>   Removes the attribute. </summary>
    ///
    /// <param name="attributeType">    Type of the attribute. </param>
    ///
    /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IModelBuilder<TClassType> RemoveAttribute(Type attributeType)
    {
        if (TypeInfo is TypeInfo typeInfo)
        {
            var att = typeInfo.FindAttribute(attributeType);

            if (att is not null)
            {
                typeInfo.RemoveAttribute(att);
            }
        }

        return this;
    }

    /// <summary>   Removes the attribute. </summary>
    ///
    /// <param name="predicate">    The predicate. </param>
    ///
    /// <returns>   Xenial.Framework.ModelBuilders.IModelBuilder&lt;TClassType&gt;. </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IModelBuilder<TClassType> RemoveAttribute(Func<Attribute, bool> predicate)
    {
        if (TypeInfo is TypeInfo typeInfo)
        {
            var att = typeInfo.Attributes.FirstOrDefault(predicate);

            if (att is not null)
            {
                typeInfo.RemoveAttribute(att);
            }
        }

        return this;
    }

    /// <summary>   Removes the attribute. </summary>
    ///
    /// <typeparam name="TAttr">    The type of the attribute. </typeparam>
    /// <param name="predicate">    (Optional) The predicate. </param>
    ///
    /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IModelBuilder<TClassType> RemoveAttribute<TAttr>(Func<TAttr, bool>? predicate = null)
        where TAttr : Attribute
    {
        var attr = FindAttribute(predicate);

        if (attr is not null && TypeInfo is TypeInfo typeInfo)
        {
            typeInfo.RemoveAttribute(attr);
        }

        return this;
    }

    /// <summary>   Finds the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
    /// <param name="predicate">    (Optional) The predicate. </param>
    ///
    /// <returns>   The found attribute. </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TAttribute? FindAttribute<TAttribute>(Func<TAttribute, bool>? predicate = null)
        where TAttribute : Attribute
        => TypeInfo.Attributes.OfType<TAttribute>().FirstOrDefault(predicate ?? (attr => true));

    /// <summary>   Configures the attribute. </summary>
    ///
    /// <typeparam name="TAttribute">   The type of the attribute. </typeparam>
    /// <param name="action">       The action. </param>
    /// <param name="predicate">    (Optional) The predicate. </param>
    ///
    /// <returns>   An IModelBuilder&lt;TClassType&gt; </returns>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IModelBuilder<TClassType> ConfigureAttribute<TAttribute>(Action<TAttribute> action, Func<TAttribute, bool>? predicate = null)
        where TAttribute : Attribute
    {
        var attr = FindAttribute(predicate);

        if (attr is not null)
        {
            action?.Invoke(attr);
        }

        return this;
    }

    /// <summary>   Fors the specified property. </summary>
    ///
    /// <exception cref="ArgumentNullException">            Thrown when one or more required
    ///                                                     arguments are null. </exception>
    /// <exception cref="CreatePropertyBuilderException">   Thrown when a Create Property Builder
    ///                                                     error condition occurs. </exception>
    /// <exception cref="InvalidOperationException">        Thrown when the requested operation is
    ///                                                     invalid. </exception>
    ///
    /// <typeparam name="TPropertyType">    The type of the property. </typeparam>
    /// <param name="propertyExpression">   The property. </param>
    ///
    /// <returns>   A PropertyBuilder&lt;TPropertyType?,TClassType&gt; </returns>

    public PropertyBuilder<TPropertyType?, TClassType> For<TPropertyType>(Expression<Func<TClassType, TPropertyType?>> propertyExpression)
    {
        _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));

        var propertyName = ExpressionHelper.Property(propertyExpression);
        if (propertyName is not null)
        {
            var memberInfo = TypeInfo.FindMember(propertyName);
            if (memberInfo is not null)
            {
                var propertyBuilder = PropertyBuilder.PropertyBuilderFor<TPropertyType?, TClassType>(memberInfo);

                Add(propertyBuilder);

                return propertyBuilder;
            }
            throw CreatePropertyBuilderException(propertyName);
        }

        throw new InvalidOperationException($"Could not create PropertyBuilder for '{TypeInfo}'.{propertyExpression} ");
    }

    /// <summary>   Fors the specified property name. </summary>
    ///
    /// <exception cref="ArgumentNullException">            nameof(propertyName) </exception>
    /// <exception cref="CreatePropertyBuilderException">   Thrown when a Create Property Builder
    ///                                                     error condition occurs. </exception>
    ///
    /// <param name="propertyName"> Name of the property. </param>
    ///
    /// <returns>   Xenial.Framework.ModelBuilders.PropertyBuilder&lt;object?, TClassType&gt;. </returns>

    public PropertyBuilder<object?, TClassType> For(string propertyName)
    {
        _ = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

        var memberInfo = TypeInfo.FindMember(propertyName);
        if (memberInfo is not null)
        {
            var propertyBuilder = PropertyBuilder.PropertyBuilderFor<object?, TClassType>(memberInfo);

            Add(propertyBuilder);

            return propertyBuilder;
        }

        throw CreatePropertyBuilderException(propertyName);
    }

    /// <summary>   Fors the specified property name. </summary>
    ///
    /// <exception cref="ArgumentNullException">            nameof(propertyName) </exception>
    /// <exception cref="CreatePropertyBuilderException">   Thrown when a Create Property Builder
    ///                                                     error condition occurs. </exception>
    ///
    /// <typeparam name="TPropertyType">    Type of the property type. </typeparam>
    /// <param name="propertyName"> Name of the property. </param>
    ///
    /// <returns>   Xenial.Framework.ModelBuilders.PropertyBuilder&lt;object?, TClassType&gt;. </returns>

    public PropertyBuilder<TPropertyType?, TClassType> For<TPropertyType>(string propertyName)
    {
        _ = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

        var memberInfo = TypeInfo.FindMember(propertyName);
        if (memberInfo is not null)
        {
            var propertyBuilder = PropertyBuilder.PropertyBuilderFor<TPropertyType?, TClassType>(memberInfo);

            Add(propertyBuilder);

            return propertyBuilder;
        }

        throw CreatePropertyBuilderException(propertyName);
    }

    private InvalidOperationException CreatePropertyBuilderException(string? propertyName) => new InvalidOperationException($"Could not create PropertyBuilder for '{TypeInfo}'.{propertyName} ");
}
