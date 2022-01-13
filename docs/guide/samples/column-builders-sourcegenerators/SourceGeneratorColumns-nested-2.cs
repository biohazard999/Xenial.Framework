/**/

namespace MainDemo.Module.BusinessObjects;

[XenialExpandMember(Constants.Address1)]
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
        Column._Address1.City with
        {
            Caption = "Address",
            Index = -1,
            GroupIndex = 0,
            SortOrder = ColumnSortOrder.Ascending
        }
        /**/
    };
}
