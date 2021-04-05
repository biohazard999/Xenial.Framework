using System;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [DetailViewLayoutBuilder]
    public class Person : XPObject
    {
        public static Layout BuildLayout()
        {
            var b = new LayoutBuilder<Person>();

            return new Layout
            {
                b.HorizontalGroup(g =>
                {
                    g.Caption = "Person";
                    g.ShowCaption = true;
                    g.RelativeSize = 25;
                },
                    b.PropertyEditor(m => m.Image, editor =>
                    {
                        editor.ShowCaption = false;
                        editor.RelativeSize = 10;
                    }),
                    b.VerticalGroup(
                        b.PropertyEditor(m => m.FullName),
                        b.HorizontalGroup(
                            b.PropertyEditor(m => m.FirstName),
                            b.PropertyEditor(m => m.LastName)
                        ),
                        b.HorizontalGroup(
                            b.PropertyEditor(m => m.Email),
                            b.PropertyEditor(m => m.Phone)
                        ),
                        b.EmptySpaceItem()
                    )
                ),
                b.TabbedGroup(
                    b.Tab("Primary Address", FlowDirection.Horizontal,
                        b.VerticalGroup(
                            b.PropertyEditor(m => m.Address1.Street, e => e.CaptionLocation = Locations.Top),
                            b.HorizontalGroup(
                                b.PropertyEditor(m => m.Address1.City, e => e.CaptionLocation = Locations.Top),
                                b.PropertyEditor(m => m.Address1.ZipPostal, e => e.CaptionLocation = Locations.Top)
                            ),
                            b.PropertyEditor(m => m.Address1.StateProvince, e => e.CaptionLocation = Locations.Top),
                            b.PropertyEditor(m => m.Address1.Country, e => e.CaptionLocation = Locations.Top),
                            b.EmptySpaceItem()
                        ),
                        b.EmptySpaceItem()
                    ),
                    b.Tab("Secondary Address", FlowDirection.Horizontal,
                        b.VerticalGroup(
                            b.PropertyEditor(m => m.Address2.Street, e => e.CaptionLocation = Locations.Top),
                            b.HorizontalGroup(
                                b.PropertyEditor(m => m.Address2.City, e => e.CaptionLocation = Locations.Top),
                                b.PropertyEditor(m => m.Address2.ZipPostal, e => e.CaptionLocation = Locations.Top)
                            ),
                            b.PropertyEditor(m => m.Address2.StateProvince, e => e.CaptionLocation = Locations.Top),
                            b.PropertyEditor(m => m.Address2.Country, e => e.CaptionLocation = Locations.Top),
                            b.EmptySpaceItem()
                        ),
                        b.EmptySpaceItem()
                    ),
                    b.Tab("Additional Addresses",
                        b.PropertyEditor(m => m.Addresses)
                    )
                )
            };
        }
    }
}