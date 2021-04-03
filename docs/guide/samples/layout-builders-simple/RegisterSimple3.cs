using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [DetailViewLayoutBuilder(typeof(PersonLayouts), nameof(BuildMyDetailViewLayout))]
    public class Person : XPObject { }

    public static class PersonLayouts
    {
        public static Layout BuildMyDetailViewLayout()
        {
            return new Layout();
        }
    }
}