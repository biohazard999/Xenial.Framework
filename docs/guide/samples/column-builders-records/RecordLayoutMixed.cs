using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [ListViewColumnsBuilder]
    public class Person : XPObject
    {
        private static ColumnsBuilder<Person> b = new();
        public static Columns BuildColumns() => new(new()
        {
            Caption = "All Persons",
            IsGroupPanelVisible = true,
            IsFooterVisible = true,
            ShowAutoFilterRow = true,
            ShowFindPanel = true,
            AutoExpandAllGroups = true
        })
        {
            b.Column(m => m.Address1.City, "Address") with
            {
                Index = -1,
                GroupIndex = 0,
                SortOrder = ColumnSortOrder.Ascending
            },
            b.Column(m => m.FirstName, 70, c =>
            {
                c.SortOrder = ColumnSortOrder.Ascending;
            }),
            new Column(nameof(LastName))
            {
                Width = 70
            },
            b.Column(m => m.Phone, 30),
            b.Column(m => m.Email, 30)
        };
    }
}
