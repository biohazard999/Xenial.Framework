﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using DevExpress.ExpressApp.Editors;

using Xenial.Framework.Layouts.Items.PubTernal;

#pragma warning disable CA1710 //Rename Type to end in Collection -> By Design
#pragma warning disable CA2227 //Collection fields should not have a setter -> By Design
#pragma warning disable CA1033 //Seal Type -> By Design

namespace Xenial.Framework.Layouts.Items.Base
{
    /// <summary>
    /// 
    /// </summary>
    [XenialCheckLicence]
    public abstract partial record LayoutItem<T>
        : LayoutItemNode,
        IEnumerableLayoutItemNode<T>,
        IEnumerableLayoutItemNode
        where T : LayoutItemNode
    {
        private LayoutItemCollection<T> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutItem"/> class.
        /// </summary>
        /// <autogeneratedoc />
        protected LayoutItem() => children = new()
        {
            Owner = this
        };

        /// <summary>
        /// Gets the is leaf.
        /// </summary>
        /// <value>The is leaf.</value>
        /// <autogeneratedoc />
        internal override bool IsLeaf => false;

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        /// <autogeneratedoc />
        public LayoutItemCollection<T> Children
        {
            get => children;
            set
            {
                if (children is not null)
                {
                    children.Owner = null;
                }

                children = value;

                if (children is not null)
                {
                    children.Owner = this;
                }
            }
        }

        int ICollection<T>.Count => Children.Count;
        bool ICollection<T>.IsReadOnly => false;
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="item">The node.</param>
        /// <autogeneratedoc />
        public void Add(T item) => Children.Add(item);

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <autogeneratedoc />
        public void Add(params T[] items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="item">The node.</param>
        /// <autogeneratedoc />
        public bool Remove(T item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            if (Children.Remove(item))
            {
                item.Parent = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <autogeneratedoc />
        public void Clear()
            => Children.Clear();

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item">The node.</param>
        /// <returns>bool.</returns>
        /// <autogeneratedoc />
        public bool Contains(T item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));
            return Children.Contains(item);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <autogeneratedoc />
        public void Add(LayoutItemNode item) => Add((T)item);

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>bool.</returns>
        /// <autogeneratedoc />
        public bool Remove(LayoutItemNode item) => Remove((T)item);
    }

    /// <summary>   (Immutable) a layout item. </summary>
    [XenialCheckLicence]
    public partial record LayoutItem : LayoutItem<LayoutItemNode>, ILayoutItemNodeWithAlign
    {
        /// <summary>   Gets or sets the horizontal align. </summary>
        ///
        /// <value> The horizontal align. </value>

        public StaticHorizontalAlign? HorizontalAlign { get; set; }

        /// <summary>   Gets or sets the vertical align. </summary>
        ///
        /// <value> The vertical align. </value>

        public StaticVerticalAlign? VerticalAlign { get; set; }
    }
}
