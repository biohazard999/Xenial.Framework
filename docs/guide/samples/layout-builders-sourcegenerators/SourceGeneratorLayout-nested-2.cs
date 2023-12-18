/**/

namespace MainDemo.Module.BusinessObjects;

[XenialExpandMember(Constants.Address1)]
[XenialExpandMember(Constants.Address2)]
public partial class PersonLayoutBuilder : LayoutBuilder<Person>
{
    public Layout BuildLayout() => new()
    {
        /**/
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
            )
            /**/
        )
    };
}

