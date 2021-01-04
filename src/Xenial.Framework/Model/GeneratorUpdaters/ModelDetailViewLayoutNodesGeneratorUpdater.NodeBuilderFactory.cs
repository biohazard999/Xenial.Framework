using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using DevExpress.ExpressApp.Model;

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

            public IModelViewLayoutElement? CreateViewLayoutElement(IModelNode parentNode, LayoutItemNode layoutItemNode)
            {
                var builder = modelViewLayoutElementFactories[layoutItemNode.GetType()].Value;

                return builder.CreateViewLayoutElement(parentNode, layoutItemNode);
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
            IModelViewLayoutElement? CreateViewLayoutElement(IModelNode parentNode, LayoutItemNode layoutItemNode);
        }

        internal abstract class ModelViewLayoutElementFactory<TModelViewLayoutElement, TLayoutItemNode> : IModelViewLayoutElementFactory
            where TModelViewLayoutElement : IModelViewLayoutElement
            where TLayoutItemNode : LayoutItemNode
        {
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
