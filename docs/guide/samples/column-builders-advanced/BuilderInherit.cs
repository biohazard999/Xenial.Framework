using System;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [ListViewColumnsBuilder(typeof(PersonColumnsBuilder))]
    public class Person : XPObject {}

    public sealed class PersonColumnsBuilder : ColumnsBuilder<Person>
    {
        public Columns BuildLayout()
        {
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
                Column(m => m.Address1.City, "Address", c =>
                {
                    c.Index = -1;
                    c.GroupIndex = 0;
                    c.SortOrder = ColumnSortOrder.Ascending;
                }),
                Column(m => m.FirstName, 70, c =>
                {
                    c.SortOrder = ColumnSortOrder.Ascending;
                }),
                Column(m => m.LastName, 70),
                Column(m => m.Phone, 30),
                Column(m => m.Email, 30)
            };
        }
    }
}
