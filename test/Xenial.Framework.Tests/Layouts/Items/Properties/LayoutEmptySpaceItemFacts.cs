using System.Drawing;

using Bogus;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;
using Xenial.Framework.Tests.Assertions;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    /// <summary>   A layout empty space item facts. </summary>
    public static class LayoutEmptySpaceItemFacts
    {
        /// <summary>   Layout empty space item tests. </summary>
        public static void LayoutEmptySpaceItemTests() => Describe(nameof(LayoutEmptySpaceItem), () =>
        {
            var faker = new Faker();
            Describe("Properties", () =>
            {
                It(nameof(IModelViewLayoutElement), () =>
                {
                    var id = faker.Random.String2(100);
                    var relativeSize = faker.Random.Double();
                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.EmptySpaceItem() with
                        {
                            Id = id,
                            RelativeSize = relativeSize,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelViewLayoutElement>((e) => new()
                    {
                        [e.Property(m => m.Id)] = id,
                        [e.Property(m => m.RelativeSize)] = relativeSize,
                    });
                });

                It($"{nameof(IModelViewLayoutElement)}2", () =>
                {
                    var id = faker.Random.String2(100);
                    var relativeSize = faker.Random.Double();
                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.EmptySpaceItem(id) with
                        {
                            RelativeSize = relativeSize,
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelViewLayoutElement>((e) => new()
                    {
                        [e.Property(m => m.Id)] = id,
                        [e.Property(m => m.RelativeSize)] = relativeSize,
                    });
                });

                It(nameof(IModelLayoutItem), () =>
                {
                    var sizeConstraintsType = faker.Random.Enum<XafSizeConstraintsType>();
                    var minSize = new Size(faker.Random.Int(), faker.Random.Int());
                    var maxSize = new Size(faker.Random.Int(), faker.Random.Int());

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.EmptySpaceItem() with
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

                It(nameof(IModelNode), () =>
                {
                    var index = faker.Random.Int();

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.EmptySpaceItem() with
                        {
                            Index = index
                        }
                    });

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, IModelNode>((e) => new()
                    {
                        [e.Property(p => p.Index)] = index
                    });
                });

                It(nameof(ISupportControlAlignment), () =>
                {
                    var horizontalAlign = faker.Random.Enum<StaticHorizontalAlign>();
                    var verticalAlign = faker.Random.Enum<StaticVerticalAlign>();

                    var detailView = CreateDetailViewWithLayout(b => new Layout
                    {
                        b.EmptySpaceItem() with
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
            });
        });
    }
}
