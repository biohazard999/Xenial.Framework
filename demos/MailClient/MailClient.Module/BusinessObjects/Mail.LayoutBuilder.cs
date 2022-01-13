
using System;
using System.Collections.Generic;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace MailClient.Module.BusinessObjects
{
    public partial class MailLayoutBuilder : LayoutBuilder<Mail>
    {
        public Layout BuildLayout() => new()
        {
            VisitPropertyEditors(ToUpper,
                VisitPropertyEditors(ToolTip,
                    VisitPropertyEditors(RowCount,
                        Editor.Account,
                        Editor.Subject,
                        Editor.From,
                        Editor.To,
                        Editor.CC,
                        Editor.AttachmentCount
                    )
                )
            ),
            EmptySpaceItem()
        };

        private void ToUpper(LayoutPropertyEditorItem item)
            => item.CaptionLocation = DevExpress.Persistent.Base.Locations.Top;

        private void ToolTip(LayoutPropertyEditorItem item)
            => item.ToolTip = "Hello World";

        private void RowCount(LayoutPropertyEditorItem item)
        {
            if (item is StringLayoutPropertyEditorItem stringItem)
            {
                stringItem.RowCount = 1;
            }
        }

        private static LayoutItemNode[] VisitPropertyEditors(Action<LayoutPropertyEditorItem> visitor, params LayoutItemNode[] nodes)
        {
            var nodesToReturn = nodes;
            foreach (var node in nodes)
            {
                foreach (var nodeToVisit in VisitNodes<LayoutPropertyEditorItem>(node))
                {
                    visitor(nodeToVisit);
                }
            }
            return nodesToReturn;
        }

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
}
