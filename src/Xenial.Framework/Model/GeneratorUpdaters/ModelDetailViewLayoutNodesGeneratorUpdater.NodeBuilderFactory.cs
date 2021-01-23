using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.Utils;

using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    public partial class ModelDetailViewLayoutNodesGeneratorUpdater
    {
        internal class NodeBuilderFactory : IModelViewLayoutElementFactory
        {
            private readonly Dictionary<Type, Lazy<IModelViewLayoutElementFactory>> modelViewLayoutElementFactories
                = new Dictionary<Type, Lazy<IModelViewLayoutElementFactory>>();

            internal NodeBuilderFactory Register<TLayoutItemNode, TModelViewLayoutElementFactory>(Func<TModelViewLayoutElementFactory> functor)
                where TLayoutItemNode : LayoutItemNode
                where TModelViewLayoutElementFactory : IModelViewLayoutElementFactory
            {
                modelViewLayoutElementFactories[typeof(TLayoutItemNode)] = new Lazy<IModelViewLayoutElementFactory>(() => functor());

                return this;
            }

            bool IModelViewLayoutElementFactory.Handles(LayoutItemNode layoutItemNode) => true;
            public IModelViewLayoutElement? CreateViewLayoutElement(IModelNode parentNode, LayoutItemNode layoutItemNode)
            {
                IModelViewLayoutElementFactory? FindFactory(Type? type)
                {
                    if (type is null)
                    {
                        return null;
                    }
                    if (modelViewLayoutElementFactories.TryGetValue(type, out var modelViewLayoutElementFactory))
                    {
                        return modelViewLayoutElementFactory.Value;
                    }
                    return FindFactory(type.GetBaseType());
                }

                var builder = FindFactory(layoutItemNode.GetType());
                if (builder is not null)
                {
                    if (builder.Handles(layoutItemNode))
                    {
                        return builder.CreateViewLayoutElement(parentNode, layoutItemNode);
                    }
                }

                return null;
            }

        }

        internal interface IViewItemFactory
        {
            IModelViewItem? CreateViewItem(IModelViewItems modelViewItems, LayoutItemLeaf layoutItemLeaf);
        }

        internal interface IViewItemFactory<TModelViewItem, TLayoutItemLeaf>
            where TModelViewItem : IModelViewItem
            where TLayoutItemLeaf : LayoutItemLeaf
        {
            /// <summary>
            /// Creates the view item.
            /// </summary>
            /// <param name="modelViewItems">The model view items.</param>
            /// <param name="layoutItemLeaf">The layout item leaf.</param>
            /// <returns>System.Nullable&lt;TModelViewItem&gt;.</returns>
            TModelViewItem? CreateViewItem(IModelViewItems modelViewItems, TLayoutItemLeaf layoutItemLeaf);
        }

        internal interface IModelViewLayoutElementFactory
        {
            bool Handles(LayoutItemNode layoutItemNode);
            IModelViewLayoutElement? CreateViewLayoutElement(IModelNode parentNode, LayoutItemNode layoutItemNode);
        }

        internal abstract class ModelViewLayoutElementFactory<TModelViewLayoutElement, TLayoutItemNode> : IModelViewLayoutElementFactory
            where TModelViewLayoutElement : IModelViewLayoutElement
            where TLayoutItemNode : LayoutItemNode
        {
            bool IModelViewLayoutElementFactory.Handles(LayoutItemNode layoutItemNode) => layoutItemNode is TLayoutItemNode;

            IModelViewLayoutElement? IModelViewLayoutElementFactory.CreateViewLayoutElement(IModelNode parentNode, LayoutItemNode layoutItemNode)
            {
                if (layoutItemNode is TLayoutItemNode tLayoutItemNode)
                {
                    return CreateViewLayoutElement(parentNode, tLayoutItemNode);
                }
                return null;
            }

            protected abstract TModelViewLayoutElement? CreateViewLayoutElement(IModelNode parentNode, TLayoutItemNode layoutItemNode);
        }

        internal interface IModelLayoutViewItemFactory
        {
            IModelLayoutViewItem CreateModelLayoutViewItem(IModelViewItem modelViewItem, IModelLayoutViewItem modelLayoutViewItem, LayoutViewItem layoutViewItem);
        }

    }
}
