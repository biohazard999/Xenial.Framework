using System;
using System.Linq;

using Bogus;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using FakeItEasy;

using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Tests.Assertions;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    /// <summary>   A layout group item facts. </summary>
    public static partial class LayoutGroupItemFacts
    {
        /// <summary>   Layout group item tests. </summary>
        public static void LayoutGroupItemTests() => Describe(nameof(LayoutGroupItem), () =>
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
                        b.LayoutGroup() with
                        {
                            ShowCaption = showCaption,
                            CaptionLocation = captionLocation,
                            CaptionHorizontalAlignment = captionHorizontalAlignment,
                            CaptionVerticalAlignment = captionVerticalAlignment,
                            CaptionWordWrap = captionWordWrap
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelLayoutGroup, IModelLayoutElementWithCaptionOptions>((e) => new()
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
                        b.LayoutGroup() with
                        {
                            Caption = caption
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelLayoutGroup, IModelLayoutElementWithCaption>((e) => new()
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
                        b.LayoutGroup() with
                        {
                            Id = id,
                            RelativeSize = relativeSize,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelLayoutGroup, IModelViewLayoutElement>((e) => new()
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
                        b.LayoutGroup() with
                        {
                            Index = index
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelLayoutGroup, IModelNode>((e) => new()
                    {
                        [e.Property(p => p.Index)] = index
                    });
                });

                It(nameof(ISupportControlAlignment), () =>
                {
                    var horizontalAlign = faker.Random.Enum<StaticHorizontalAlign>();
                    var verticalAlign = faker.Random.Enum<StaticVerticalAlign>();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.LayoutGroup() with
                        {
                            HorizontalAlign = horizontalAlign,
                            VerticalAlign = verticalAlign
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, ISupportControlAlignment>((e) => new()
                    {
                        [e.Property(p => p.HorizontalAlign)] = horizontalAlign,
                        [e.Property(p => p.VerticalAlign)] = verticalAlign,
                    });
                });

                It(nameof(IModelLayoutGroup), () =>
                {
                    var imageName = faker.Random.String();
                    var flowDirection = faker.Random.Enum<FlowDirection>();
                    var isCollapsibleGroup = faker.Random.Bool();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.LayoutGroup() with
                        {
                            ImageName = imageName,
                            Direction = flowDirection,
                            IsCollapsibleGroup = isCollapsibleGroup
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelLayoutGroup, IModelLayoutGroup>((e) => new()
                    {
                        [e.Property(p => p.ImageName)] = imageName,
                        [e.Property(p => p.Direction)] = flowDirection,
                        [e.Property(p => p.IsCollapsibleGroup)] = isCollapsibleGroup,
                    });
                });

                It(nameof(IModelToolTip), () =>
                {
                    var toolTip = faker.Random.String();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.LayoutGroup() with
                        {
                            ToolTip = toolTip
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelLayoutGroup, IModelToolTip>((e) => new()
                    {
                        [e.Property(p => p.ToolTip)] = toolTip
                    });
                });

                It(nameof(IModelToolTipOptions), () =>
                {
                    var toolTipTitle = faker.Random.String();
                    var toolTipIconType = faker.Random.Enum<ToolTipIconType>();

                    var detailView = CreateDetailViewWithLayout(b => new()
                    {
                        b.LayoutGroup() with
                        {
                            ToolTipTitle = toolTipTitle,
                            ToolTipIconType = toolTipIconType,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelLayoutGroup, IModelToolTipOptions>((e) => new()
                    {
                        [e.Property(p => p.ToolTipTitle)] = toolTipTitle,
                        [e.Property(p => p.ToolTipIconType)] = toolTipIconType
                    });
                });
            });
        });
    }
}
