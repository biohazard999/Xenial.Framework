using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using Bogus;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;

using Shouldly;

using Xenial.Data;
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

            File.WriteAllText(@"C:\F\tmp\Xenial\1.html", html);
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
                It(nameof(IModelLayoutElementWithCaptionOptions), () =>
                {
                    var showCaption = faker.Random.Bool();
                    var captionLocation = faker.Random.Enum<DevExpress.Persistent.Base.Locations>();
                    var captionHorizontalAlignment = faker.Random.Enum<DevExpress.Utils.HorzAlignment>();
                    var captionVerticalAlignment = faker.Random.Enum<DevExpress.Utils.VertAlignment>();
                    var captionWordWrap = faker.Random.Enum<DevExpress.Utils.WordWrap>();

                    var model = CreateApplication(new[]
                    {
                        typeof(LayoutPropertyEditorItemBusinessObject)
                    },
                    typesInfo =>
                    {
                        ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                            .WithDetailViewLayout(b => new Layout
                            {
                                b.PropertyEditor(m => m.StringProperty) with
                                {
                                    ShowCaption = showCaption,
                                    CaptionLocation = captionLocation,
                                    CaptionHorizontalAlignment = captionHorizontalAlignment,
                                    CaptionVerticalAlignment = captionVerticalAlignment,
                                    CaptionWordWrap = captionWordWrap
                                }
                            })
                        .Build();
                    });

                    var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();

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

                    var model = CreateApplication(new[]
                    {
                        typeof(LayoutPropertyEditorItemBusinessObject)
                    },
                    typesInfo =>
                    {
                        ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                            .WithDetailViewLayout(b => new Layout
                            {
                                b.PropertyEditor(m => m.StringProperty) with
                                {
                                    HorizontalAlign = horizontalAlign,
                                    VerticalAlign = verticalAlign
                                }
                            })
                        .Build();
                    });

                    var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();

                    detailView.AssertLayoutItemProperties<IModelViewLayoutElement, ISupportControlAlignment>((e) => new()
                    {
                        [e.Property(p => p.HorizontalAlign)] = horizontalAlign,
                        [e.Property(p => p.VerticalAlign)] = verticalAlign,
                    });
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
                                HorizontalAlign = DevExpress.ExpressApp.Editors.StaticHorizontalAlign.NotSet
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
