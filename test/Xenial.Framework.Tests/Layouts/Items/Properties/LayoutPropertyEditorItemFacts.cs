using System;
using System.Drawing;
using System.Linq;

using Bogus;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using FakeItEasy;

using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

using static Xenial.Framework.Tests.Layouts.Items.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    public static class LayoutPropertyEditorItemFacts
    {
        public static void LayoutPropertyEditorItemTests() => Describe(nameof(LayoutPropertyEditorItem), () =>
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

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            ShowCaption = showCaption,
                            CaptionLocation = captionLocation,
                            CaptionHorizontalAlignment = captionHorizontalAlignment,
                            CaptionVerticalAlignment = captionVerticalAlignment,
                            CaptionWordWrap = captionWordWrap
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelLayoutElementWithCaptionOptions>((e) => new()
                    {
                        [e.Property(p => p.ShowCaption)] = showCaption,
                        [e.Property(p => p.CaptionLocation)] = captionLocation,
                        [e.Property(p => p.CaptionHorizontalAlignment)] = captionHorizontalAlignment,
                        [e.Property(p => p.CaptionVerticalAlignment)] = captionVerticalAlignment,
                        [e.Property(p => p.CaptionWordWrap)] = captionWordWrap,
                    });
                });

                It(nameof(IModelViewLayoutElement), () =>
                {
                    var id = faker.Random.String2(100);
                    var relativeSize = faker.Random.Double();
                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(p => p.StringProperty) with
                        {
                            Id = id,
                            RelativeSize = relativeSize
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelViewLayoutElement>((e) => new()
                    {
                        [e.Property(m => m.Id)] = id,
                        [e.Property(m => m.RelativeSize)] = relativeSize,
                    });
                });

                It(nameof(ISupportControlAlignment), () =>
                {
                    var horizontalAlign = faker.Random.Enum<StaticHorizontalAlign>();
                    var verticalAlign = faker.Random.Enum<StaticVerticalAlign>();

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
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

                It(nameof(IModelLayoutItem), () =>
                {
                    var sizeConstraintsType = faker.Random.Enum<XafSizeConstraintsType>();
                    var minSize = new Size(faker.Random.Int(), faker.Random.Int());
                    var maxSize = new Size(faker.Random.Int(), faker.Random.Int());

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            SizeConstraintsType = sizeConstraintsType,
                            MinSize = minSize,
                            MaxSize = maxSize,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelLayoutItem>((e) => new()
                    {
                        [e.Property(p => p.SizeConstraintsType)] = sizeConstraintsType,
                        [e.Property(p => p.MinSize)] = minSize,
                        [e.Property(p => p.MaxSize)] = maxSize,
                    });
                });

                It(nameof(IModelToolTip), () =>
                {
                    var toolTip = faker.Random.String();

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            ToolTip = toolTip
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelToolTip>((e) => new()
                    {
                        [e.Property(p => p.ToolTip)] = toolTip
                    });
                });

                It(nameof(IModelToolTipOptions), () =>
                {
                    var toolTipTitle = faker.Random.String();
                    var toolTipIconType = faker.Random.Enum<ToolTipIconType>();

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            ToolTipTitle = toolTipTitle,
                            ToolTipIconType = toolTipIconType,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelToolTipOptions>((e) => new()
                    {
                        [e.Property(p => p.ToolTipTitle)] = toolTipTitle,
                        [e.Property(p => p.ToolTipIconType)] = toolTipIconType
                    });
                });

                It(nameof(IModelNode), () =>
                {
                    var index = faker.Random.Int();

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            Index = index,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelNode>((e) => new()
                    {
                        [e.Property(p => p.Index)] = index
                    });
                });

                It(nameof(IModelViewLayoutElement), () =>
                {
                    var id = faker.Random.String();

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            Id = id,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelViewLayoutElement>((e) => new()
                    {
                        [e.Property(p => p.Id)] = id
                    });
                });

                It($"{nameof(LayoutPropertyEditorItem.PropertyEditorOptions)} get called", () =>
                {
                    var optionsCallback = A.Fake<Action<IModelPropertyEditor>>();
                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            PropertyEditorOptions = optionsCallback
                        }
                    });

                    var _ = detailView?.Layout?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                    A.CallTo(optionsCallback).MustHaveHappenedOnceExactly();
                });

                It($"{nameof(LayoutPropertyEditorItem.ViewItemOptions)} get called", () =>
                {
                    var optionsCallback = A.Fake<Action<IModelViewItem>>();
                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.PropertyEditor(m => m.StringProperty) with
                        {
                            ViewItemOptions = optionsCallback
                        }
                    });

                    var _ = detailView?.Layout?.FirstOrDefault(); //We need to access the layout node cause it's lazy evaluated

                    A.CallTo(optionsCallback).MustHaveHappenedOnceExactly();
                });
            });
        });
    }
}
