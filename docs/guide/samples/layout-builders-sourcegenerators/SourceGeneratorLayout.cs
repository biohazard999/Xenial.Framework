using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace MainDemo.Module.BusinessObjects;

[Persistent]
[DefaultClassOptions]
[DetailViewLayoutBuilder(typeof(PersonLayoutBuilder))]
public class Person : XPObject { }

[XenialExpandMember(Constants.Address1)]
[XenialExpandMember(Constants.Address2)]
public partial class PersonLayoutBuilder : LayoutBuilder<Person>
{
    public Layout BuildLayout() => new()
    {
        HorizontalGroup("Person") with
        {
            ShowCaption = true,
            RelativeSize = 25,
            Children = new()
            {
                Editor.Image with
                {
                    ShowCaption = false,
                    RelativeSize = 10
                },
                VerticalGroup(
                    Editor.FullName,
                    HorizontalGroup(
                        Editor.FirstName,
                        Editor.LastName
                    ),
                    HorizontalGroup(
                        Editor.Email,
                        Editor.Phone
                    ),
                    EmptySpaceItem()
                )
            }
        },
        TabbedGroup(
            Tab("Primary Address", FlowDirection.Horizontal,
                VerticalGroup(
                    Editor._Address1.Street with { CaptionLocation = Locations.Top },
                    HorizontalGroup(
                        Editor._Address1.City with { CaptionLocation = Locations.Top },
                        Editor._Address1.ZipPostal with { CaptionLocation = Locations.Top }
                    ),
                    Editor._Address1.StateProvince with { CaptionLocation = Locations.Top },
                    Editor._Address1.Country with { CaptionLocation = Locations.Top },
                    EmptySpaceItem()
                ),
                EmptySpaceItem()
            ),
            Tab("Secondary Address", FlowDirection.Horizontal,
                VerticalGroup(
                    Editor._Address2.Street with { CaptionLocation = Locations.Top },
                    HorizontalGroup(
                        Editor._Address2.City with { CaptionLocation = Locations.Top },
                        Editor._Address2.ZipPostal with { CaptionLocation = Locations.Top }
                    ),
                    Editor._Address2.StateProvince with { CaptionLocation = Locations.Top },
                    Editor._Address2.Country with { CaptionLocation = Locations.Top },
                    EmptySpaceItem()
                ),
                EmptySpaceItem()
            ),
            Tab("Additional Addresses",
                Editor.Addresses
            )
        )
    };
}

