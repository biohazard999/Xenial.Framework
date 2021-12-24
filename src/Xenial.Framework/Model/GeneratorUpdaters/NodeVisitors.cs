using System;
using System.Collections.Generic;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Model.GeneratorUpdaters;

internal static class NodeVisitors
{
    internal static IEnumerable<TItem> VisitNodes<TItem>(LayoutItemNode node)
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
