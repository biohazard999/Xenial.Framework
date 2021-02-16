using System;
using System.Linq;

using Bogus;

using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;

using FakeItEasy;

using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Tests.Assertions;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    public static partial class LayoutTabbedGroupItemFacts
    {
        public static void LayoutTabbedGroupItemTests() => Describe(nameof(LayoutTabbedGroupItem), () =>
        {
            var faker = new Faker();
            Describe("Properties", () =>
            {
                It(nameof(IModelLayoutElementWithCaptionOptions), () =>
                {
                    var showCaption = faker.Random.Bool();
                    var captionLocation = faker.Random.Enum<DevExpress.Persistent.Base.Locations>();
                    var captionHorizontalAlignment = faker.Random.Enum<DevExpress.Utils.HorzAlignment>();
                    var captionVerticalAlignment = faker.Random.Enum<DevExpress.Utils.VertAlignment>();
                    var captionWordWrap = faker.Random.Enum<DevExpress.Utils.WordWrap>();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.TabbedGroup() with
                        {
                            ShowCaption = showCaption,
                            CaptionLocation = captionLocation,
                            CaptionHorizontalAlignment = captionHorizontalAlignment,
                            CaptionVerticalAlignment = captionVerticalAlignment,
                            CaptionWordWrap = captionWordWrap
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelTabbedGroup, IModelLayoutElementWithCaptionOptions>((e) => new()
                    {
                        [e.Property(p => p.ShowCaption)] = showCaption,
                        [e.Property(p => p.CaptionLocation)] = captionLocation,
                        [e.Property(p => p.CaptionHorizontalAlignment)] = captionHorizontalAlignment,
                        [e.Property(p => p.CaptionVerticalAlignment)] = captionVerticalAlignment,
                        [e.Property(p => p.CaptionWordWrap)] = captionWordWrap,
                    });
                });

                It(nameof(IModelLayoutElementWithCaption), () =>
                {
                    var caption = faker.Random.String();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.TabbedGroup() with
                        {
                            Caption = caption
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelTabbedGroup, IModelLayoutElementWithCaption>((e) => new()
                    {
                        [e.Property(p => p.Caption)] = caption,
                    });
                });

                It(nameof(IModelViewLayoutElement), () =>
                {
                    var id = faker.Random.String2(100);
                    var relativeSize = faker.Random.Double();
                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.TabbedGroup() with
                        {
                            Id = id,
                            RelativeSize = relativeSize,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelTabbedGroup, IModelViewLayoutElement>((e) => new()
                    {
                        [e.Property(m => m.Id)] = id,
                        [e.Property(m => m.RelativeSize)] = relativeSize,
                    });
                });

                It(nameof(IModelNode), () =>
                {
                    var index = faker.Random.Int();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.TabbedGroup() with
                        {
                            Index = index
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelTabbedGroup, IModelNode>((e) => new()
                    {
                        [e.Property(p => p.Index)] = index
                    });
                });

                It(nameof(IModelTabbedGroup), () =>
                {
                    var flowDirection = faker.Random.Enum<FlowDirection>();
                    var multiLine = faker.Random.Bool();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.TabbedGroup() with
                        {
                            Direction = flowDirection,
                            MultiLine = multiLine
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelTabbedGroup, IModelTabbedGroup>((e) => new()
                    {
                        [e.Property(p => p.Direction)] = flowDirection,
                        [e.Property(p => p.MultiLine)] = multiLine,
                    });
                });
            });


            It($"{nameof(LayoutTabbedGroupItem.TabbedGroupOptions)} is called", () =>
            {
                var optionsCallback = A.Fake<Action<IModelTabbedGroup>>();
                var detailView = CreateDetailViewWithLayout(b => new()
                {
                    b.TabbedGroup() with
                    {
                        TabbedGroupOptions = optionsCallback
                    }
                });

                var _ = detailView?.Layout?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                A.CallTo(optionsCallback).MustHaveHappenedOnceExactly();
            });
        });
    }
}
