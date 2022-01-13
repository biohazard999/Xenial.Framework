using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace MainDemo.Module.BusinessObjects;

[Persistent]
[DefaultClassOptions]
[ListViewColumnsBuilder(typeof(PersonColumnsBuilder))]
public class Person : XPObject { }

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
        },
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
