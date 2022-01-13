using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.Data;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects;

public partial class EmployeeColumnsBuilder : ColumnsBuilder<Employee>
{

    /* TODO: Filtering
* <Filters CurrentFilterId="AllEmployees" IsNewNode="True">
<Filter Id="AllEmployees" Caption="All Employees" IsNewNode="True" />
<Filter Id="Managers" Criteria="Contains([Position.Title], 'Manager')" IsNewNode="True" />
<Filter Id="TopExecutives" Caption="Top Executives" Criteria="Contains([Position.Title], 'President') Or Contains([Position.Title], 'Director') Or StartsWith([Position.Title], 'Chief') And EndsWith([Position.Title], 'Officer')" IsNewNode="True" />
<Filter Id="Developers" Criteria="Position.Title = 'Developer'" IsNewNode="True" />
</Filters>
*/
    public Columns BuildColumns() => new(new()
    {
        Caption = "Employees",
        IsGroupPanelVisible = true,
        AutoExpandAllGroups = true,
    })
    {
        Column.Department with { Index = -1, GroupIndex = 0 },
        Column.TitleOfCourtesy,
        Column.FirstName,
        Column.LastName with { SortIndex = 0, SortOrder = ColumnSortOrder.Ascending, Width = 100 },
        Column.Position with { SortOrder = ColumnSortOrder.None, SortIndex = -1 },
        Column.Email,
        Column.Birthday
    };

    public Columns BuildAllColumns() => new(new()
    {
        Caption = "Employees",
        IsGroupPanelVisible = false,
        IsFooterVisible = true,
        AllowEdit = false
    })
    {
        Column.TitleOfCourtesy with { Width = 70 },
        Column.FirstName with { Width = 70 },
        Column.LastName with { Width = 100, SortIndex = 0, SortOrder = ColumnSortOrder.Ascending },
        Column.Position with { Width = 70 },
        Column.Department with { Width = 70 },
        Column.Email with { Width = 70 },
        Column.Birthday with { Width = 70 },
        Column.Address1 with { Width = 70 }
    };

    public Columns BuildVariantColumns() => new();

    public Columns BuildLookupColumns() => new()
    {
        Column.LastName with { SortOrder = ColumnSortOrder.Ascending }
    };
}
