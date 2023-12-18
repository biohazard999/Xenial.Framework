using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [ListViewColumnsBuilder]
    public class Person : XPObject
    {
        public static Columns BuildColumns()
        {
            return new Columns();
        }
    }
}
