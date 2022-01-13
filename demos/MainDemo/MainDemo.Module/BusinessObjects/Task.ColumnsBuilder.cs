using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects;

public partial class DemoTaskColumnsBuilder : ColumnsBuilder<DemoTask>
{
    /* TODO: Summary
     * <ColumnInfo Id="ActualWorkHours" Index="-1">
          <Summary>
            <ColumnSummaryItem Id="Summary0" Index="0" SummaryType="Sum" IsNewNode="True" />
          </Summary>
        </ColumnInfo>
    */
    public Columns BuildColumns() => new(new()
    {
        Caption = "Tasks",
        AllowEdit = true,
        NewItemRowPosition = DevExpress.ExpressApp.NewItemRowPosition.Top,
        IsFooterVisible = false,
        ShowAutoFilterRow = true,
    })
    {
        Column.ActualWorkHours with { Index = -1 },
        Column.Status,
        Column.AssignedTo,
        Column.StartDate,
        Column.DueDate,
        Column.Priority
    };
}
