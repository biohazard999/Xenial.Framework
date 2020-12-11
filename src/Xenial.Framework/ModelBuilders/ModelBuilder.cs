using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Data;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelBuilder
    {
        /// <summary>
        /// Finds the type information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typesInfo">The types information.</param>
        /// <returns></returns>
        public static ITypeInfo FindTypeInfo<T>(this ITypesInfo typesInfo)
        {
            _ = typesInfo ?? throw new ArgumentNullException(nameof(typesInfo));
            return typesInfo.FindTypeInfo(typeof(T));
        }

        /// <summary>
        /// Creates the specified types information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typesInfo">The types information.</param>
        /// <returns></returns>
        public static ModelBuilder<T> Create<T>(ITypesInfo typesInfo)
            => new(typesInfo.FindTypeInfo<T>());

        /// <summary>
        /// Creates the specified types information.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="typesInfo">The types information.</param>
        /// <returns></returns>
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
    public class ModelBuilder<TClassType> : BuilderManager, ITypeInfoProvider, IModelBuilder<TClassType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBuilder{T}"/> class.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        public ModelBuilder(ITypeInfo typeInfo) => TypeInfo = typeInfo;

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <value>
        /// The type information.
        /// </value>
        public ITypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public Type TargetType { get; } = typeof(TClassType);

        /// <summary>
        /// Gets the exp.
        /// </summary>
        /// <value>
        /// The exp.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ExpressionHelper<TClassType> ExpressionHelper { get; } = new ExpressionHelper<TClassType>();

        /// <summary>
        /// Gets the default detail view.
        /// </summary>
        /// <value>
        /// The default detail view.
        /// </value>
        public virtual string DefaultDetailView => ModelNodeIdHelper.GetDetailViewId(typeof(TClassType));

        /// <summary>
        /// Gets the default ListView.
        /// </summary>
        /// <value>
        /// The default ListView.
        /// </value>
        public virtual string DefaultListView => ModelNodeIdHelper.GetListViewId(typeof(TClassType));

        /// <summary>
        /// Gets the default lookup ListView.
        /// </summary>
        /// <value>
        /// The default lookup ListView.
        /// </value>
        public virtual string DefaultLookupListView => ModelNodeIdHelper.GetLookupListViewId(typeof(TClassType));

        /// <summary>
        /// Nesteds the ListView identifier.
        /// </summary>
        /// <typeparam name="TRet">The type of the ret.</typeparam>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        public virtual string NestedListViewId<TRet>(Expression<Func<TClassType, TRet>> expr)
            where TRet : IEnumerable
                => ModelNodeIdHelper.GetNestedListViewId(typeof(TClassType), ExpressionHelper.Property(expr));

        /// <summary>
        /// Withes the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns></returns>
        public IModelBuilder<TClassType> WithAttribute(Attribute attribute)
        {
            TypeInfo.AddAttribute(attribute);
            return this;
        }

        /// <summary>
        /// Withes the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="attribute">The attribute.</param>
        /// <param name="configureAction">The configure action.</param>
        /// <returns></returns>
        public IModelBuilder<TClassType> WithAttribute<TAttribute>(TAttribute attribute, Action<TAttribute>? configureAction = null)
            where TAttribute : Attribute
        {
            configureAction?.Invoke(attribute);
            return WithAttribute((Attribute)attribute);
        }

        /// <summary>
        /// Withes the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="configureAction">The configure action.</param>
        /// <returns></returns>
        public IModelBuilder<TClassType> WithAttribute<TAttribute>(Action<TAttribute>? configureAction = null)
            where TAttribute : Attribute, new()
            => WithAttribute(new TAttribute(), configureAction);

        /// <summary>
        /// Removes the attribute.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IModelBuilder<TClassType> RemoveAttribute(Type attributeType)
        {
            if (TypeInfo is TypeInfo typeInfo)
            {
                var att = typeInfo.FindAttribute(attributeType);

                if (att != null)
                {
                    typeInfo.RemoveAttribute(att);
                }
            }

            return this;
        }

        /// <summary>
        /// Removes the attribute.
        /// </summary>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IModelBuilder<TClassType> RemoveAttribute<TAttr>(Func<TAttr, bool>? predicate = null)
            where TAttr : Attribute
        {
            var attr = FindAttribute(predicate);

            if (attr != null && TypeInfo is TypeInfo typeInfo)
            {
                typeInfo.RemoveAttribute(attr);
            }

            return this;
        }

        /// <summary>
        /// Finds the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TAttribute? FindAttribute<TAttribute>(Func<TAttribute, bool>? predicate = null)
            where TAttribute : Attribute
            => TypeInfo.Attributes.OfType<TAttribute>().FirstOrDefault(predicate ?? (attr => true));

        /// <summary>
        /// Configures the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Fors the specified property.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public PropertyBuilder<TPropertyType, TClassType> For<TPropertyType>(Expression<Func<TClassType, TPropertyType>> property)
        {
            _ = property ?? throw new ArgumentNullException(nameof(property));

            var builder = PropertyBuilder.PropertyBuilderFor<TPropertyType, TClassType>(TypeInfo.FindMember(ExpressionHelper.Property(property)));

            Add(builder);

            return builder;
        }
    }
}
