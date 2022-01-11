using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [ListViewColumnsBuilder(typeof(PersonColumnsBuilder), nameof(BuildMyListViewColumns))]
    public class Person : XPObject { }

    public static class PersonColumnsBuilder
    {
        public static Columns BuildMyListViewColumns()
        {
            return new Columns();
        }
    }
}
