using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Markdig;

using Xenial.Framework.Utils;

namespace Xenial.FeatureCenter.Module.HtmlBuilders
{
    internal static class HtmlBuilder
    {
        private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

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
    }
}
