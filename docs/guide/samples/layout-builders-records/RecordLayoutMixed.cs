using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace MainDemo.Module.BusinessObjects
{
    [Persistent]
    [DefaultClassOptions]
    [DetailViewLayoutBuilder]
    public class Person : XPObject
    {
        private static readonly LayoutBuilder<Person> b = new();
        public static Layout BuildLayout() => new()
        {
            b.HorizontalGroup() with
            {
                Caption = "Person",
                ShowCaption = true,
                RelativeSize = 25,
                Children = new()
                {
                    b.PropertyEditor(m => m.Image) with
                    {
                        ShowCaption = false,
                        RelativeSize = 10,
                    },
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
                }
            },
            new LayoutTabbedGroupItem
            {
                b.Tab("Primary Address", FlowDirection.Horizontal,
                    b.VerticalGroup(
                        b.PropertyEditor(m => m.Address1.Street) with { CaptionLocation = Locations.Top },
                        b.HorizontalGroup(
                            b.PropertyEditor(m => m.Address1.City) with { CaptionLocation = Locations.Top },
                            b.PropertyEditor(m => m.Address1.ZipPostal) with { CaptionLocation = Locations.Top }
                        ),
                        b.PropertyEditor(m => m.Address1.StateProvince) with { CaptionLocation = Locations.Top },
                        b.PropertyEditor(m => m.Address1.Country) with { CaptionLocation = Locations.Top },
                        b.EmptySpaceItem()
                    ),
                    b.EmptySpaceItem()
                ),
                b.Tab("Secondary Address", FlowDirection.Horizontal,
                    b.VerticalGroup() with
                    {
                        Children = new()
                        {
                            b.PropertyEditor(m => m.Address2.Street, e => e.CaptionLocation = Locations.Top),
                            b.HorizontalGroup() with
                            {
                                Children = new()
                                {
                                    b.PropertyEditor(m => m.Address2.City, e => e.CaptionLocation = Locations.Top),
                                    b.PropertyEditor(m => m.Address2.ZipPostal, e => e.CaptionLocation = Locations.Top)
                                }
                            },
                            b.PropertyEditor(m => m.Address2.StateProvince, e => e.CaptionLocation = Locations.Top),
                            b.PropertyEditor(m => m.Address2.Country, e => e.CaptionLocation = Locations.Top),
                            b.EmptySpaceItem()
                        }
                    },
                    b.EmptySpaceItem()
                ),
                b.Tab(
                    b.PropertyEditor(m => m.Addresses)
                ) with { Caption = "Additional Addresses" }
            }
        };
    }
}