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
                b.Column(m => m.Address1.City, column =>
                {
                    column.Caption = "Address";
                    column.Index = -1;
                    column.GroupIndex = 0;
                    column.SortOrder = ColumnSortOrder.Ascending;
                }),
                b.Column(m => m.FirstName, column =>
                {
                    column.SortOrder = ColumnSortOrder.Ascending;
                    column.Width = 70;
                }),
                b.Column(m => m.LastName, column => column.Width = 70),
                b.Column(m => m.Phone, column => column.Width = 30),
                b.Column(m => m.Email, column => column.Width = 30)
            };
        }
    }
}
