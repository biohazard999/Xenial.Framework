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
                    b.VerticalGroup() with
                    {
                        Children = new()
                        {
                            b.PropertyEditor(m => m.FullName),
                            b.HorizontalGroup() with
                            {
                                Children = new()
                                {
                                    b.PropertyEditor(m => m.FirstName),
                                    b.PropertyEditor(m => m.LastName)
                                }
                            },
                            b.HorizontalGroup() with
                            {
                                Children = new()
                                {
                                    b.PropertyEditor(m => m.Email),
                                    b.PropertyEditor(m => m.Phone)
                                }
                            },
                            b.EmptySpaceItem()
                        }
                    }
                }
            },
            b.TabbedGroup() with
            {
                Children = new()
                {
                    b.Tab("Primary Address", FlowDirection.Horizontal) with
                    {
                        Children = new()
                        {
                            b.VerticalGroup() with
                            {
                                Children = new()
                                {
                                    b.PropertyEditor(m => m.Address1.Street, e => e.CaptionLocation = Locations.Top),
                                    b.HorizontalGroup() with
                                    {
                                        Children = new()
                                        {
                                            b.PropertyEditor(m => m.Address1.City, e => e.CaptionLocation = Locations.Top),
                                            b.PropertyEditor(m => m.Address1.ZipPostal, e => e.CaptionLocation = Locations.Top)
                                        }
                                    },
                                    b.PropertyEditor(m => m.Address1.StateProvince, e => e.CaptionLocation = Locations.Top),
                                    b.PropertyEditor(m => m.Address1.Country, e => e.CaptionLocation = Locations.Top),
                                    b.EmptySpaceItem()
                                }
                            },
                            b.EmptySpaceItem()
                        }
                    },
                    b.Tab("Secondary Address", FlowDirection.Horizontal) with
                    {
                        Children = new()
                        {
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
                        }
                    },
                    b.Tab("Additional Addresses") with
                    {
                        Children = new()
                        {
                            b.PropertyEditor(m => m.Addresses)
                        }
                    }
                }
            }
        };
    }
}