using System;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [DetailViewLayoutBuilder(typeof(PersonLayoutBuilder))]
    public class Person : XPObject {}

    public sealed class PersonLayoutBuilder : LayoutBuilder<Person>
    {
        public Layout BuildLayout()
        {
            return new Layout
            {
                HorizontalGroup(g =>
                {
                    g.Caption = "Person";
                    g.ShowCaption = true;
                    g.RelativeSize = 25;
                },
                    PropertyEditor(m => m.Image, editor =>
                    {
                        editor.ShowCaption = false;
                        editor.RelativeSize = 10;
                    }),
                    VerticalGroup(
                        PropertyEditor(m => m.FullName),
                        HorizontalGroup(
                            PropertyEditor(m => m.FirstName),
                            PropertyEditor(m => m.LastName)
                        ),
                        HorizontalGroup(
                            PropertyEditor(m => m.Email),
                            PropertyEditor(m => m.Phone)
                        ),
                        EmptySpaceItem()
                    )
                ),
                TabbedGroup(
                    Tab("Primary Address", FlowDirection.Horizontal,
                        VerticalGroup(
                            PropertyEditor(m => m.Address1.Street, e => e.CaptionLocation = Locations.Top),
                            HorizontalGroup(
                                PropertyEditor(m => m.Address1.City, e => e.CaptionLocation = Locations.Top),
                                PropertyEditor(m => m.Address1.ZipPostal, e => e.CaptionLocation = Locations.Top)
                            ),
                            PropertyEditor(m => m.Address1.StateProvince, e => e.CaptionLocation = Locations.Top),
                            PropertyEditor(m => m.Address1.Country, e => e.CaptionLocation = Locations.Top),
                            EmptySpaceItem()
                        ),
                        EmptySpaceItem()
                    ),
                    Tab("Secondary Address", FlowDirection.Horizontal,
                        VerticalGroup(
                            PropertyEditor(m => m.Address2.Street, e => e.CaptionLocation = Locations.Top),
                            HorizontalGroup(
                                PropertyEditor(m => m.Address2.City, e => e.CaptionLocation = Locations.Top),
                                PropertyEditor(m => m.Address2.ZipPostal, e => e.CaptionLocation = Locations.Top)
                            ),
                            PropertyEditor(m => m.Address2.StateProvince, e => e.CaptionLocation = Locations.Top),
                            PropertyEditor(m => m.Address2.Country, e => e.CaptionLocation = Locations.Top),
                            EmptySpaceItem()
                        ),
                        EmptySpaceItem()
                    ),
                    Tab("Additional Addresses",
                        PropertyEditor(m => m.Addresses)
                    )
                )
            };
        }
    }
}