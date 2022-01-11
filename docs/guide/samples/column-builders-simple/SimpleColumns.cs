using DevExpress.Data;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;

namespace DXApplication6.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [ListViewColumnsBuilder]
    public class Person : XPObject
    {
        public static Columns BuildColumns()
        {
            return new Columns(new ListViewOptions()
            {
                Caption = "All Persons",
                IsGroupPanelVisible = true,
                IsFooterVisible = true,
                ShowAutoFilterRow = true,
                ShowFindPanel = true,
                AutoExpandAllGroups = true
            })
            {
                new Column($"{nameof(Address1)}.{nameof(Address.City)}")
                {
                    Caption = "Address",
                    Index = -1,
                    GroupIndex = 0,
                    SortOrder = ColumnSortOrder.Ascending
                },
                new Column(nameof(FirstName))
                {
                    SortOrder = ColumnSortOrder.Ascending,
                    Width = 70
                },
                new Column(nameof(LastName))
                {
                    Width = 70
                },
                new Column(nameof(Phone))
                {
                    Width = 30
                },
                new Column(nameof(Email))
                {
                    Width = 30
                },
            };
        }
    }
}
