using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.ColumnItems;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    internal static class ModelColumnsBuilderNodesGeneratorUpdaterMappers
    {
        internal static void MapColumn(
            IModelColumn modelColumn,
            Column column
        )
        {
            if (column.Width.HasValue)
            {
                modelColumn.Width
                    = column.Width ?? modelColumn.Width;
            }
            if (column.SortOrder.HasValue)
            {
                modelColumn.SortOrder
                    = column.SortOrder ?? modelColumn.SortOrder;
            }
            if (column.SortIndex.HasValue)
            {
                modelColumn.SortIndex
                    = column.SortIndex ?? modelColumn.SortIndex;
            }
            if (column.GroupIndex.HasValue)
            {
                modelColumn.GroupIndex
                    = column.GroupIndex ?? modelColumn.GroupIndex;
            }
            if (column.GroupInterval.HasValue)
            {
                modelColumn.GroupInterval
                    = column.GroupInterval ?? modelColumn.GroupInterval;
            }
        }
    }
}
