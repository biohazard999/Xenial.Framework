using System;

using Xenial;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace Acme.Module.BusinessObjects
{
    [XenialExpandMember(Constants.Address1)]
    [XenialExpandMember(Constants.Address2)]
    [XenialExpandMember(Constants._Address1.Country)]
    [XenialExpandMember(Constants._Address2.Country)]
    public partial class PersonLayout : LayoutBuilder<Person>
    {
        /* Build to look at Person.Layout.PersonLayout.g.cs */

        public Layout BuildLayout() => new()
        {
            Editor.FullName,
            HorizontalGroup(
                Editor.FirstName,
                Editor.LastName
            ),
            HorizontalGroup(
                Editor.DateOfBirth,
                EmptySpaceItem()
            ),
            TabbedGroup(
                Tab(Constants.Address1,
                    VerticalGroup(
                        Editor._Address1.Street,
                        Editor._Address1.City,
                        Editor._Address1._Country.CountryName
                    )
                ),
                Tab(Constants.Address2,
                    VerticalGroup(
                        Editor._Address2.Street,
                        Editor._Address2.City,
                        Editor._Address2._Country.CountryName
                    )
                )
            )
        };
    }
}
