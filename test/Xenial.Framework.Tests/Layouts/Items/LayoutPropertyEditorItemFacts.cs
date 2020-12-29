using System;
using System.IO;
using System.Net;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;

using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Tests.Assertions.Xml;

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
        public static void LayoutPropertyEditorItemTests() => FDescribe(nameof(LayoutPropertyEditorItem), () =>
        {
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
                                ShowCaption = true,
                                CaptionLocation = DevExpress.Persistent.Base.Locations.Top,
                                CaptionHorizontalAlignment = DevExpress.Utils.HorzAlignment.Near,
                                CaptionVerticalAlignment = DevExpress.Utils.VertAlignment.Bottom,
                                CaptionWordWrap = DevExpress.Utils.WordWrap.NoWrap
                            }
                        })
                    .Build();
                });

                var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();

                var xml = UserDifferencesHelper.GetUserDifferences(detailView)[""];
                var prettyXml = new XmlFormatter().Format(xml);
                var encode = WebUtility.HtmlEncode(prettyXml);
                var html = @$"
<html>
    <head>
        <link href=""https://unpkg.com/prismjs@1.22.0/themes/prism-okaidia.css"" rel=""stylesheet"" />
    </head>
    <body style='background-color: #272822; color: #bbb; font-family: sans-serif; margin: 0; padding: 0;'>
        <h1 style='text-align: center; margin-top: .5rem'>XAF Layout Inspector</h1>
        <hr style='border: none; border-top: 1px solid #bbb;' />
        <pre><code class='language-xml'>{encode}</code></pre>
        <script src=""https://unpkg.com/prismjs@1.22.0/components/prism-core.min.js""></script>
        <script src=""https://unpkg.com/prismjs@1.22.0/plugins/autoloader/prism-autoloader.min.js""></script>
    </body>
</html>";

#if DEBUG
                File.WriteAllText(@"C:\F\tmp\Xenial\1.html", html);
#endif
            });
        });
    }
}
