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
        public static Layout BuildLayout()
        {
            return new Layout
            {
                new HorizontalLayoutGroupItem
                {
                    Caption = "Person",
                    ShowCaption = true,
                    RelativeSize = 25,
                    Children =
                    {
                        new LayoutPropertyEditorItem(nameof(Image))
                        {
                            ShowCaption = false,
                            RelativeSize = 10
                        },
                        new VerticalLayoutGroupItem
                        {
                            new LayoutPropertyEditorItem(nameof(FullName)),
                            new HorizontalLayoutGroupItem
                            {
                                new LayoutPropertyEditorItem(nameof(FirstName)),
                                new LayoutPropertyEditorItem(nameof(LastName)),
                            },
                            new HorizontalLayoutGroupItem
                            {
                                new LayoutPropertyEditorItem(nameof(Email)),
                                new LayoutPropertyEditorItem(nameof(Phone)),
                            },
                            new LayoutEmptySpaceItem(),
                        }
                    }
                },
                new LayoutTabbedGroupItem
                {
                    new LayoutTabGroupItem("Primary Address", FlowDirection.Horizontal)
                    {
                        new VerticalLayoutGroupItem
                        {
                            new LayoutPropertyEditorItem($"{nameof(Address1)}.{nameof(Address.Street)}")
                            {
                                CaptionLocation = Locations.Top
                            },
                            new HorizontalLayoutGroupItem
                            {
                                new LayoutPropertyEditorItem($"{nameof(Address1)}.{nameof(Address.City)}")
                                {
                                    CaptionLocation = Locations.Top
                                },
                                new LayoutPropertyEditorItem($"{nameof(Address1)}.{nameof(Address.ZipPostal)}")
                                {
                                    CaptionLocation = Locations.Top
                                },
                            },
                            new LayoutPropertyEditorItem($"{nameof(Address1)}.{nameof(Address.StateProvince)}")
                            {
                                CaptionLocation = Locations.Top
                            },
                            new LayoutPropertyEditorItem($"{nameof(Address1)}.{nameof(Address.Country)}")
                            {
                                CaptionLocation = Locations.Top
                            },
                            new LayoutEmptySpaceItem(),
                        },
                        new LayoutEmptySpaceItem(),
                    },
                    new LayoutTabGroupItem("Secondary Address", FlowDirection.Horizontal)
                    {
                        new VerticalLayoutGroupItem
                        {
                            new LayoutPropertyEditorItem($"{nameof(Address2)}.{nameof(Address.Street)}")
                            {
                                CaptionLocation = Locations.Top
                            },
                            new HorizontalLayoutGroupItem
                            {
                                new LayoutPropertyEditorItem($"{nameof(Address2)}.{nameof(Address.City)}")
                                {
                                    CaptionLocation = Locations.Top
                                },
                                new LayoutPropertyEditorItem($"{nameof(Address2)}.{nameof(Address.ZipPostal)}")
                                {
                                    CaptionLocation = Locations.Top
                                },
                            },
                            new LayoutPropertyEditorItem($"{nameof(Address2)}.{nameof(Address.StateProvince)}")
                            {
                                CaptionLocation = Locations.Top
                            },
                            new LayoutPropertyEditorItem($"{nameof(Address2)}.{nameof(Address.Country)}")
                            {
                                CaptionLocation = Locations.Top
                            },
                            new LayoutEmptySpaceItem(),
                        },
                        new LayoutEmptySpaceItem(),
                    },
                    new LayoutTabGroupItem("Additional Addresses")
                    {
                        new LayoutPropertyEditorItem(nameof(Addresses))
                    }
                }
            };
        }
    }
}