/**/

namespace MainDemo.Module.BusinessObjects
{
    [XenialExpandMember(Constants.Address1)]
    public partial class PersonColumnsBuilder : ColumnsBuilder<Person>
    {
        public Columns BuildColumns() => new()
        {
            /**/
        };
    }
}
