using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Markdig;

using Xenial.Framework.Utils;

namespace Xenial.FeatureCenter.Module.BusinessObjects
{
    [NonPersistent]
    public abstract class FeatureCenterDemoBaseObjectId : FeatureCenterBaseObjectId
    {
        private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        protected FeatureCenterDemoBaseObjectId(Session session) : base(session) { }

        protected virtual IEnumerable<RequiredNuget> GetRequiredModules() => Array.Empty<RequiredNuget>();

        private string BuildInstallationMarkDown()
        {
            var sb = new StringBuilder();

            var types = GetRequiredModules().Where(m => !m.Platform.HasValue || (m.Platform == FeatureCenterModule.CurrentPlatform));

            sb.AppendLine(Section.Create(string.Empty, new TabGroup
            {
                Tabs = new()
                {
                    new(".NET CLI", "fas fa-terminal")
                    {
                        HtmlAble = CodeBlock.Create(
                            "shell",
                            string.Join(Environment.NewLine, types.Select(t => $"dotnet add package {t.Nuget} --version {XenialVersion.Version}"))
                        )
                    },
                    new("PackageReference", "fas fa-code")
                    {
                        HtmlAble = CodeBlock.Create(
                            "xml",
                            string.Join(Environment.NewLine, types.Select(t => $"<PackageReference Include=\"{t.Nuget}\" Version=\"{XenialVersion.Version}\" />"))
                        )
                    },
                    new("Package Manager", "fas fa-terminal")
                    {
                        HtmlAble = CodeBlock.Create(
                            "powershell",
                            string.Join(Environment.NewLine, types.Select(t => $"Install-Package {t.Nuget} -Version {XenialVersion.Version}"))
                        )
                    },
                    new("Paket CLI", "fas fa-terminal")
                    {
                        HtmlAble = CodeBlock.Create(
                            "shell",
                            string.Join(Environment.NewLine, types.Select(t => $"paket add {t.Nuget} --version {XenialVersion.Version}"))
                        )
                    }
                }
            }).ToString());

            AddInstallationSection(sb);

            return sb.ToString();
        }

        protected virtual void AddInstallationSection(StringBuilder sb) { }

        internal record Tab(string Caption, string? Image)
        {
            public IHtmlAble HtmlAble { get; set; } = HtmlBlock.Create(string.Empty);

            public void ToPill(StringBuilder sb, bool isFirst)
            {
                var active = isFirst ? "is-active" : string.Empty;
                sb.AppendLine($"<li class='{active}'>");

                if (string.IsNullOrEmpty(Image))
                {
                    sb.AppendLine($"<a>{Caption}</a>");
                }
                else
                {
                    sb.AppendLine($"<a><span class='icon is-small'><i class='{Image}' aria-hidden='true'></i></span><span>{Caption}</span></a>");
                }
                sb.AppendLine("</li>");
            }

            public void ToTabPane(StringBuilder sb, bool isFirst)
            {
                var active = isFirst ? "is-active" : string.Empty;
                sb.AppendLine($"<li class='{active}'>");
                sb.AppendLine(HtmlAble.ToString());
                sb.AppendLine("</li>");
            }
        }

        internal record Section(string? Header = null) : IHtmlAble
        {
            public static Section Create(string header, IHtmlAble htmlAble)
                => new(header)
                {
                    Content = new()
                    {
                        htmlAble
                    }
                };

            public List<IHtmlAble> Content { get; set; } = new List<IHtmlAble>();

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.AppendLine("<div class='block'>");
                if (!string.IsNullOrEmpty(Header))
                {
                    sb.AppendLine($"<h2 class='subtitle'>{Header}</h2>");
                }

                foreach (var item in Content)
                {
                    sb.AppendLine(item.ToString());
                }

                sb.AppendLine("</div>");

                return sb.ToString();
            }
        }

        internal interface IHtmlAble
        {
            string ToString();
        }

        internal record HtmlBlock(string Html) : IHtmlAble
        {
            internal static HtmlBlock Create(string html)
                => new HtmlBlock(html);

            public override string ToString()
                => Html;
        }

        internal record MarkDownBlock(string MarkDown) : IHtmlAble
        {
            internal static MarkDownBlock FromResourceString(string path)
            {
                var markdown = ResourceUtil.GetResourceString(typeof(MarkDownBlock), path);
                return new MarkDownBlock(markdown);
            }

            public override string ToString()
                => Markdown.ToHtml(MarkDown, pipeline);
        }

        internal record CodeBlock(string Type, string Code) : IHtmlAble
        {
            public static CodeBlock Create(string type, string code)
                => new(type, code);

            public override string ToString()
            {
                var code = $@"```{Type}
{Code}
```";
                return Markdown.ToHtml(code, pipeline);
            }
        }

        internal record ImageBlock(string Size, Stream Stream) : IHtmlAble
        {
            public string MimeType { get; set; } = "image/gif";

            public static ImageBlock Create(string size, string path)
            {
                var stream = ResourceUtil.GetResourceStream(typeof(ImageBlock), path);
                return new ImageBlock(size, stream);
            }

            public static ImageBlock Create(string size, byte[] bytes)
            {
                var stream = new MemoryStream(bytes);
                return new ImageBlock(size, stream);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"<div class='is-flex is-justify-content-center'>");
                using (Stream)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        Stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }

                    (int width, int height) GetSize()
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            Stream.Position = 0;
                            Stream.CopyTo(memoryStream);
                            using var image = System.Drawing.Image.FromStream(memoryStream);
                            return (image.Width, image.Height);
                        }
                    }


                    var base64 = Convert.ToBase64String(bytes);
                    var (width, height) = GetSize();
                    sb.AppendLine($"<img src='data:{MimeType};base64,{base64}' width='{width}' height='{height}' />");
                }
                sb.AppendLine("</div>");

                return sb.ToString();
            }
        }

        internal record TabGroup : IHtmlAble
        {
            public List<Tab> Tabs { get; set; } = new List<Tab>();

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("<div class='tabs-wrapper'>");

                sb.Append("<div class='tabs'>");
                sb.AppendLine("<ul>");
                foreach (var tab in Tabs)
                {
                    var isFirst = Tabs.First() == tab;
                    tab.ToPill(sb, isFirst);
                }
                sb.AppendLine("</ul>");
                sb.AppendLine("</div>");

                sb.AppendLine("<div class='tabs-content'>");
                sb.AppendLine("<ul>");
                foreach (var tab in Tabs)
                {
                    var isFirst = Tabs.First() == tab;
                    tab.ToTabPane(sb, isFirst);
                }
                sb.AppendLine("</ul>");
                sb.AppendLine("</div>");

                sb.AppendLine("</div>");
                return sb.ToString();
            }
        }

        private string BuildInstallationHtml()
        {
            var markdown = BuildInstallationMarkDown();
            return BuildHtml("Installation", markdown);
        }

        protected virtual string UsageHtml { get; } = string.Empty;

        private string BuildUsageHtml()
        {
            if (!string.IsNullOrEmpty(UsageHtml))
            {
                return BuildHtml("Usage", UsageHtml);
            }
            return string.Empty;
        }

        protected string BuildHtml(string title, string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            var html = $@"<!doctype html>

<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">

  <title>Xenial.FeatureCenter</title>
  <meta name=""description"" content=""Xenial.FeatureCenter"" >
  <meta name=""author"" content=""Manuel Grundner"">
  <link href='https://unpkg.com/prismjs@1.23.0/themes/prism-okaidia.css' rel='stylesheet' />
  <link href='https://unpkg.com/prismjs@1.23.0/plugins/toolbar/prism-toolbar.css' rel='stylesheet' />
  <link href='https://unpkg.com/bulma@0.9.1/css/bulma.min.css' rel='stylesheet' />
<style>

:root {{
  --xenial-light-color: #8b93a1;
  --xenial-darker-color: #333333;
}}

/* The emerging W3C standard
   that is currently Firefox-only */
* {{
  scrollbar-width: thin;
  scrollbar-color: var(--xenial-light-color) var(--xenial-darker-color);
}}

/* Works on Chrome/Edge/Safari */
*::-webkit-scrollbar {{
  width: 12px;
  height: 12px;
}}

*::-webkit-scrollbar-corner {{
  background-color: var(--xenial-darker-color);
}}

*::-webkit-scrollbar-track {{
  background: var(--xenial-darker-color);
}}
*::-webkit-scrollbar-thumb {{
  background-color: var(--xenial-light-color);
  border-radius: 20px;
  border: 3px solid var(--xenial-darker-color);
}}

.tabs-content li {{
    display: none;
    list-style: none;
}}

.tabs-content li.is-active {{
    display: block;
}}

pre code .tag:not(body) {{
    background-color: unset;
    align-items: unset;
    background-color: unset;
    border-radius: unset;
    color: unset;
    display: unset;
    font-size: unset;
    height: unset;
    justify-content: unset;
    line-height: unset;
    padding-left: unset;
    padding-right: unset;
    white-space: unset;
}}

</style>
</head>

<body>
  <section class='section'>
    <div class='container'>
    <h1 class='title'>{ title }</h1>
    { Markdown.ToHtml(markdown, pipeline)}
    </div>
  </section>
  <script defer src='https://use.fontawesome.com/releases/v5.14.0/js/all.js'></script>
  <script src='https://unpkg.com/@vizuaalog/bulmajs@0.12.0/dist/bulma.js'></script>
  <script src='https://unpkg.com/clipboard@2/dist/clipboard.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-core.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-clike.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-markup-templating.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-javascript.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-typescript.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-csharp.min'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-powershell.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-markup.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-yaml.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-json.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-php.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-css.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-css-extras.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-sass.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-scss.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/toolbar/prism-toolbar.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/copy-to-clipboard/prism-copy-to-clipboard.min.js'></script>  
</body>
</html>";

            return html;
        }

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        public string InstallationMarkdown => BuildInstallationMarkDown();

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias("Xenial.WebViewStringPropertyEditor")]
        public string Installation => BuildInstallationHtml();

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias("Xenial.WebViewStringPropertyEditor")]
        public string Usage => BuildUsageHtml();

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias("Xenial.WebViewStringPropertyEditor")]
        public string Remarks => BuildHtml("Remarks", RemarksHtml());

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias("Xenial.WebViewStringPropertyEditor")]
        public string SupportedPlatforms => BuildHtml("Supported Platforms", SupportedPlatformsHtml());

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [WebViewUriEditor]
        public Uri DemoCode => new Uri($"https://github.com/xenial-io/Xenial.Framework/blob/{XenialVersion.Branch}/{DemoCodeFileName}");

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [WebViewUriEditor]
        public Uri Documentation => new Uri($"https://docs.xenial.io/{DocsUrlFragment}");

        protected virtual string DemoCodeFileName => string.Empty;
        protected virtual string DocsUrlFragment => string.Empty;
        protected virtual string RemarksHtml() => string.Empty;
        protected virtual string SupportedPlatformsHtml() => string.Empty;
    }


    public record RequiredNuget(string ModuleName, AvailablePlatform? Platform = null)
    {
        private readonly string nugetPostFix = Platform.HasValue ? $".{Platform.Value}" : string.Empty;
        public string Nuget => $"Xenial.Framework.{ModuleName}{nugetPostFix}";
    }

    public enum AvailablePlatform
    {
        Win,
        Blazor
    }
}
