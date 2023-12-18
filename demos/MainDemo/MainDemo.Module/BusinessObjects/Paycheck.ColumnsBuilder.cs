using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.Data;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects;

public partial class PaycheckColumnsBuilder : ColumnsBuilder<Paycheck>
{
    public Columns BuildColumns() => new(new()
    {
        Caption = "Payroll",
        IsGroupPanelVisible = true,
        ShowAutoFilterRow = true
    })
    {
        Column.Employee with { SortOrder = ColumnSortOrder.Ascending },
        Column.PayPeriod with { SortOrder = ColumnSortOrder.Ascending, GroupIndex = 0 },
        Column.PayPeriodStart,
        Column.PayPeriodEnd,
        Column.PayRate,
        Column.Hours,
        Column.OvertimePayRate,
        Column.OvertimeHours,
        Column.TaxRate,
        Column.TotalTax,
        Column.GrossPay,
        Column.NetPay
    };
}
