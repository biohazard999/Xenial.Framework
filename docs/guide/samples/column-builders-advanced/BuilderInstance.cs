using System;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [DetailViewLayoutBuilder]
    public class Person : XPObject
    {
        public static Columns BuildColumns()
        {
            var b = new ColumnsBuilder<Person>();

            return new Columns(new ListViewOptions
            {
                Caption = "All Persons",
                IsGroupPanelVisible = true,
                IsFooterVisible = true,
                ShowAutoFilterRow = true,
                ShowFindPanel = true,
                AutoExpandAllGroups = true
            })
            {
                b.Column(m => m.Address1.City, "Address", c =>
                {
                    c.Index = -1;
                    c.GroupIndex = 0;
                    c.SortOrder = ColumnSortOrder.Ascending;
                }),
                b.Column(m => m.FirstName, 70, c =>
                {
                    c.SortOrder = ColumnSortOrder.Ascending;
                }),
                b.Column(m => m.LastName, 70),
                b.Column(m => m.Phone, 30),
                b.Column(m => m.Email, 30)
            };
        }
    }
}
