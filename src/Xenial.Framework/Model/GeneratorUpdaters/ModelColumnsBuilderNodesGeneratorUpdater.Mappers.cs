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

        internal static void MapModelMemberViewItem(
            IModelMemberViewItem modelColumn,
            Column column
        )
        {
            modelColumn.PropertyName
                = column.PropertyName ?? modelColumn.PropertyName;

            if (column.DataSourceProperty is not null)
            {
                modelColumn.DataSourceProperty
                    = column.DataSourceProperty;
            }
            if (column.DataSourceCriteriaProperty is not null)
            {
                modelColumn.DataSourceCriteriaProperty
                    = column.DataSourceCriteriaProperty;
            }
            if (column.MaxLength.HasValue)
            {
                modelColumn.MaxLength
                    = column.MaxLength ?? modelColumn.MaxLength;
            }
            if (column.ImageEditorCustomHeight.HasValue)
            {
                modelColumn.ImageEditorCustomHeight
                    = column.ImageEditorCustomHeight ?? modelColumn.ImageEditorCustomHeight;
            }
            if (column.ImageEditorMode.HasValue)
            {
                modelColumn.ImageEditorMode
                    = column.ImageEditorMode ?? modelColumn.ImageEditorMode;
            }
            if (column.ImageEditorFixedWidth.HasValue)
            {
                modelColumn.ImageEditorFixedWidth
                    = column.ImageEditorFixedWidth ?? modelColumn.ImageEditorFixedWidth;
            }
            if (column.ImageEditorFixedHeight.HasValue)
            {
                modelColumn.ImageEditorFixedHeight
                    = column.ImageEditorFixedHeight ?? modelColumn.ImageEditorFixedHeight;
            }
        }

        internal static void MapModelLayoutElement(
           IModelLayoutElement modelColumn,
           Column column
        )
        {
            if (column.Index.HasValue)
            {
                modelColumn.Index
                    = column.Index ?? modelColumn.Index;
            }
        }

        internal static void MapModelToolTip(
          IModelToolTip modelColumn,
          Column column
        )
        {
            if (column.ToolTip is not null)
            {
                modelColumn.ToolTip = column.ToolTip;
            }
        }

        internal static void MapModelCommonMemberViewItem(
          IModelCommonMemberViewItem modelColumn,
          Column column
        )
        {
            if (column.NullText is not null)
            {
                modelColumn.NullText = column.NullText;
            }
            if (column.DataSourceCriteria is not null)
            {
                modelColumn.DataSourceCriteria = column.DataSourceCriteria;
            }
            if (column.EditMaskType.HasValue)
            {
                modelColumn.EditMaskType =
                    column.EditMaskType ?? modelColumn.EditMaskType;
            }
            if (column.IsPassword.HasValue)
            {
                modelColumn.IsPassword =
                    column.IsPassword ?? modelColumn.IsPassword;
            }
            if (column.DisplayFormat is not null)
            {
                modelColumn.DisplayFormat = column.DisplayFormat;
            }
            if (column.Caption is not null)
            {
                modelColumn.Caption = column.Caption;
            }
            if (column.RowCount.HasValue)
            {
                modelColumn.RowCount =
                    column.RowCount ?? modelColumn.RowCount;
            }
            if (column.AllowEdit.HasValue)
            {
                modelColumn.AllowEdit =
                    column.AllowEdit ?? modelColumn.AllowEdit;
            }
            if (column.LookupProperty is not null)
            {
                modelColumn.LookupProperty = column.LookupProperty;
            }
            if (column.DataSourcePropertyIsNullMode.HasValue)
            {
                modelColumn.DataSourcePropertyIsNullMode =
                    column.DataSourcePropertyIsNullMode ?? modelColumn.DataSourcePropertyIsNullMode;
            }
            if (column.DataSourcePropertyIsNullCriteria is not null)
            {
                modelColumn.DataSourcePropertyIsNullCriteria = column.DataSourcePropertyIsNullCriteria;
            }
            if (column.AllowClear.HasValue)
            {
                modelColumn.AllowClear =
                    column.AllowClear ?? modelColumn.AllowClear;
            }
            if (column.CaptionForTrue is not null)
            {
                modelColumn.CaptionForTrue = column.CaptionForTrue;
            }
            if (column.CaptionForFalse is not null)
            {
                modelColumn.CaptionForFalse = column.CaptionForFalse;
            }
            if (column.ImageForTrue is not null)
            {
                modelColumn.ImageForTrue = column.ImageForTrue;
            }
            if (column.ImageForFalse is not null)
            {
                modelColumn.ImageForFalse = column.ImageForFalse;
            }
            if (column.ImageSizeMode.HasValue)
            {
                modelColumn.ImageSizeMode =
                    column.ImageSizeMode ?? modelColumn.ImageSizeMode;
            }
            if (column.PredefinedValues is not null)
            {
                modelColumn.PredefinedValues = column.PredefinedValues;
            }
            if (column.LookupEditorMode.HasValue)
            {
                modelColumn.LookupEditorMode =
                    column.LookupEditorMode ?? modelColumn.LookupEditorMode;
            }
            if (column.ImmediatePostData.HasValue)
            {
                modelColumn.ImmediatePostData =
                    column.ImmediatePostData ?? modelColumn.ImmediatePostData;
            }
            if (column.PropertyEditorType is not null)
            {
                modelColumn.PropertyEditorType = column.PropertyEditorType;
            }
            if (column.EditMask is not null)
            {
                modelColumn.EditMask = column.EditMask;
            }
        }
    }
}
