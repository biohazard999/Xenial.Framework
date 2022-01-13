/**/

namespace MainDemo.Module.BusinessObjects;

public partial class PersonLayoutBuilder : LayoutBuilder<Person>
{
    public partial class PersonColumnsBuilder : ColumnsBuilder<Person>
    {
        public Columns BuildColumns() => new(new()
        {
            Caption = "All Persons",
            IsGroupPanelVisible = true,
            IsFooterVisible = true,
            ShowAutoFilterRow = true,
            ShowFindPanel = true,
            AutoExpandAllGroups = true
        })
        {
            /**/
            Column.FirstName with
            {
                Width = 70,
                SortOrder = ColumnSortOrder.Ascending
            },
            Column.LastName with { Width = 70 },
            Column.Phone with { Width = 30 },
            Column.Email with { Width = 30 }
        };
    }
}
