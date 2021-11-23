using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.Layouts.Items.Base;

#pragma warning disable CA1710 //Rename Type to end in Collection -> By Design
#pragma warning disable CA2227 //Collection fields should not have a setter: By Design

namespace Xenial.Framework.Layouts.Items;

/// <summary>   Class LayoutItemCollection. </summary>
///
/// <typeparam name="T">    . </typeparam>
///
/// <seealso cref="ICollection{T}"/>
/// <seealso cref="IEnumerable{T}"/>

public sealed class LayoutItemCollection<T> : ICollection<T>, IEnumerable<T>
    where T : LayoutItemNode
{
    private readonly LinkedList<T> innerList;
    private LayoutItemNode? owner;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutItemCollection{T}"/> class.
    /// </summary>

    public LayoutItemCollection() => innerList = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutItemCollection{T}"/> class.
    /// </summary>
    ///
    /// <param name="childNodes">   The child nodes. </param>

    public LayoutItemCollection(IEnumerable<T> childNodes) => innerList = new(childNodes);

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutItemCollection{T}"/> class.
    /// </summary>
    ///
    /// <param name="childNodes">   The child nodes. </param>

    public LayoutItemCollection(params T[] childNodes) => innerList = new(childNodes);

    internal LayoutItemNode? Owner
    {
        get => owner;
        set
        {
            foreach (var child in this.ToArray())
            {
                child.Parent = null;
            }

            owner = value;

            foreach (var child in this.ToArray())
            {
                child.Parent = owner;
            }
        }
    }

    /// <summary>
    /// Gets the number of elements contained in the
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    /// </summary>
    ///
    /// <value> The count. </value>

    public int Count => innerList.Count;

    bool ICollection<T>.IsReadOnly => false;

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="item"> The object to add to the
    ///                     <see cref="T:System.Collections.Generic.ICollection`1"></see>. </param>

    public void Add(T item)
    {
        _ = item ?? throw new ArgumentNullException(nameof(item));

        if (Owner is not null)
        {
            if (!Owner.Equals(item.Parent))
            {
                item.Parent = Owner;
            }
        }

        if (!innerList.Contains(item))
        {
            innerList.AddLast(item);
        }
    }

    /// <summary>   Adds the specified items. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="items">    The items. </param>

    public void AddRange(IEnumerable<T> items)
    {
        _ = items ?? throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    /// </summary>

    public void Clear()
    {
        while (innerList.Count > 0
            && innerList.First?.Value is T itemNode
        )
        {
            itemNode.ParentItem = null;
            innerList.Remove(itemNode);
        }
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains
    /// a specific value.
    /// </summary>
    ///
    /// <param name="item"> The object to locate in the
    ///                     <see cref="T:System.Collections.Generic.ICollection`1"></see>. </param>
    ///
    /// <returns>
    /// true if <paramref name="item">item</paramref> is found in the
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
    /// </returns>

    public bool Contains(T item) => innerList.Contains(item);

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to
    /// an <see cref="T:System.Array"></see>, starting at a particular
    /// <see cref="T:System.Array"></see> index.
    /// </summary>
    ///
    /// <param name="array">        The one-dimensional <see cref="T:System.Array"></see> that is the
    ///                             destination of the elements copied from
    ///                             <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///                             The <see cref="T:System.Array"></see> must have zero-based
    ///                             indexing. </param>
    /// <param name="arrayIndex">   The zero-based index in array at which copying begins. </param>

    public void CopyTo(T[] array, int arrayIndex) => innerList.CopyTo(array, arrayIndex);

    /// <summary>   Returns an enumerator that iterates through the collection. </summary>
    ///
    /// <returns>   An enumerator that can be used to iterate through the collection. </returns>

    public IEnumerator<T> GetEnumerator() => innerList.GetEnumerator();

    /// <summary>
    /// Removes the first occurrence of a specific object from the
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    /// </summary>
    ///
    /// <param name="item"> The object to remove from the
    ///                     <see cref="T:System.Collections.Generic.ICollection`1"></see>. </param>
    ///
    /// <returns>
    /// true if <paramref name="item">item</paramref> was successfully removed from the
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method
    /// also returns false if <paramref name="item">item</paramref> is not found in the original
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    /// </returns>

    public bool Remove(T item) => innerList.Remove(item);

    IEnumerator IEnumerable.GetEnumerator() => innerList.GetEnumerator();
}
