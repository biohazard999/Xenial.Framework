/**/

namespace MainDemo.Module.BusinessObjects;

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
                /**/
                EmptySpaceItem()
            ),
            Tab("Secondary Address", FlowDirection.Horizontal,
                /**/
                EmptySpaceItem()
            ),
            Tab("Additional Addresses",
                Editor.Addresses
            )
        )
    };
}
