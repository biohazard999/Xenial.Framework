using System;
using System.Collections.Generic;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// 
/// </summary>
public static class NodeVisitors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<TItem> VisitNodes<TItem>(LayoutItemNode node)
          where TItem : LayoutItemNode
    {
        if (node is TItem targetNode)
        {
            yield return targetNode;
        }

        if (node is IEnumerable<LayoutItemNode> items)
        {
            foreach (var item in items)
            {
                foreach (var nestedItem in VisitNodes<TItem>(item))
                {
                    yield return nestedItem;
                }
            }
        }
    }
}
