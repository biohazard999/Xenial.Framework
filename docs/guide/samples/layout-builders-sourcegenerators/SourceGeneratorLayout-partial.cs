using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [DetailViewLayoutBuilder(typeof(PersonLayoutBuilder))]
    public class Person : XPObject { }

    public partial class PersonLayoutBuilder : LayoutBuilder<Person>
    {
        public Layout BuildLayout() => new();
    }
}
