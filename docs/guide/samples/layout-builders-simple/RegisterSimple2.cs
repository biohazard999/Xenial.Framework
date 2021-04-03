using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [DetailViewLayoutBuilder(nameof(BuildMyDetailViewLayout))]
    public class Person : XPObject
    {
        public static Layout BuildMyDetailViewLayout()
        {
            return new Layout();
        }
    }
}