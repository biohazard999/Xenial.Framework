using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [LookupListViewColumnsBuilder]
    public class Person : XPObject
    {
        public static Columns BuildLookupColumns()
        {
            return new Columns();
        }
    }
}
