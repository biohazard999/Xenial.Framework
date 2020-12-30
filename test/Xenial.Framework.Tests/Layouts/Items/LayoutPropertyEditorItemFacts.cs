using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

using Bogus;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;

using FakeItEasy;

using Shouldly;

using Xenial.Data;
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Tests.Assertions.Xml;
using Xenial.Utils;

using static Xenial.Framework.Tests.Layouts.Items.TestModelApplicationFactory;
using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    [DomainComponent]
    public sealed class LayoutPropertyEditorItemBusinessObject
    {
        public string? StringProperty { get; set; }
    }

    public static class LayoutPropertyEditorItemFacts
    {
        internal static void VisualizeModelNode(this IModelNode? modelNode)
        {
            _ = modelNode ?? throw new ArgumentNullException(nameof(modelNode));
            var xml = UserDifferencesHelper.GetUserDifferences(modelNode)[""];
            var prettyXml = new XmlFormatter().Format(xml);
            var encode = WebUtility.HtmlEncode(prettyXml);
            var html = @$"
<html>
    <head>
        <link href='https://unpkg.com/prismjs@1.22.0/themes/prism-okaidia.css' rel='stylesheet' />
    </head>
    <body style='background-color: #272822; color: #bbb; font-family: sans-serif; margin: 0; padding: 0;'>
        <h1 style='text-align: center; margin-top: .5rem'>XAF Layout Inspector</h1>
        <hr style='border: none; border-top: 1px solid #bbb;' />
        <pre><code class='language-xml'>{encode}</code></pre>
        <script src='https://unpkg.com/prismjs@1.22.0/components/prism-core.min.js'></script>
        <script src='https://unpkg.com/prismjs@1.22.0/plugins/autoloader/prism-autoloader.min.js'></script>
    </body>
</html>";
#if DEBUG
            File.WriteAllText(@"C:\F\tmp\Xenial\1.html", html);
#endif
        }

        internal static void AssertLayoutItemProperties<TModelType, TTargetModelType>(this IModelDetailView? modelDetailView, Func<ExpressionHelper<TTargetModelType>, Dictionary<string, object>> asserter)
            where TModelType : IModelViewLayoutElement
        {
            modelDetailView.ShouldSatisfyAllConditions(
                () => modelDetailView.ShouldNotBeNull(),
                () => modelDetailView!.Layout.ShouldNotBeNull(),
                () => modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName].ShouldNotBeNull(),
                () => modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName].ShouldBeAssignableTo<IModelLayoutGroup>()
            );

            var mainLayoutGroupNode = (IModelLayoutGroup)modelDetailView!.Layout[ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName];
            var targetNode = mainLayoutGroupNode.GetNodes<TModelType>().FirstOrDefault();

            targetNode.ShouldNotBeNull();

            var helper = ExpressionHelper.Create<TTargetModelType>();
            var assertions = asserter(helper);

            var hasValueAssertions = assertions
                .Select(a => new Action(
                    () => targetNode!.HasValue(a.Key)
                )).ToArray();

            targetNode.ShouldSatisfyAllConditions(hasValueAssertions);

            var valueAssertions = assertions
                .Select(a => new Action(
                    () => targetNode!
                        .GetValue<object>(a.Key)
                        .ShouldBe(a.Value, $"'{a.Key}' should be '{a.Value}' but was not.")
                    )
                ).ToArray();

            targetNode.ShouldSatisfyAllConditions(valueAssertions);
        }

        public static void LayoutPropertyEditorItemTests() => FDescribe(nameof(LayoutPropertyEditorItem), () =>
        {
            var faker = new Faker();

            Describe("Properties", () =>
            {
                IModelDetailView? CreateDetailViewWithLayout(Func<LayoutBuilder<LayoutPropertyEditorItemBusinessObject>, Layout> layoutFunctor)
                {
                    var model = CreateApplication(new[]
                    {
                        typeof(LayoutPropertyEditorItemBusinessObject)
                    },
                    typesInfo =>
                    {
                        ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                            .WithDetailViewLayout(layoutFunctor)
                        .Build();
                    });

                    var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();
                    return detailView;
                }

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

            It("gets created with ModelBuilder", () =>
            {
                var model = CreateApplication(new[]
                {
                    typeof(LayoutPropertyEditorItemBusinessObject)
                },
                typesInfo =>
                {
                    ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                        .WithDetailViewLayout(p => new Layout
                        {
                            p.PropertyEditor(m => m.StringProperty) with
                            {
                                ShowCaption = false,
                                CaptionLocation = DevExpress.Persistent.Base.Locations.Top,
                                CaptionHorizontalAlignment = DevExpress.Utils.HorzAlignment.Near,
                                CaptionVerticalAlignment = DevExpress.Utils.VertAlignment.Bottom,
                                CaptionWordWrap = DevExpress.Utils.WordWrap.NoWrap,
                                HorizontalAlign = DevExpress.ExpressApp.Editors.StaticHorizontalAlign.NotSet,
                                ToolTip = "My Tooltip"
                            }
                        })
                    .Build();
                });

                var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();

                detailView.VisualizeModelNode();
            });
        });
    }
}
