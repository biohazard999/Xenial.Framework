/**/

namespace MainDemo.Module.BusinessObjects
{
    [XenialExpandMember(Constants.Address1)]
    [XenialExpandMember(Constants.Address2)]
    public partial class PersonLayoutBuilder : LayoutBuilder<Person>
    {
        public Layout BuildLayout() => new()
        {
            /**/
        };
    }
}
