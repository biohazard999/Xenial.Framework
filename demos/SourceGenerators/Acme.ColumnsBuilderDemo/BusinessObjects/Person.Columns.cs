using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xenial.Framework.Layouts;

namespace Acme.Module.BusinessObjects;

[Xenial.XenialExpandMember(Constants.Address1)]
[Xenial.XenialExpandMember(Constants._Address1.Country)]
public partial class PersonColumnsBuilder : ColumnsBuilder<Person>
{
    public Columns BuildColumns() => new()
    {
        Column.FirstName,
        Column.LastName,
        Column._Address1.Street,
        Column._Address1.City,
        Column._Address1._Country.CountryName with { Caption = "Country" }
    };
}
