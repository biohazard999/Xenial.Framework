using System;
using System.Collections.Generic;
using System.Text;

using static Xenial.FeatureCenter.Module.HtmlBuilders.HtmlBuilder;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [FeatureStatusLab]
    public partial class WebViewHtmlStringEditorDemo
    {
        protected override string DemoCodeFileName => "demos/FeatureCenter/Xenial.FeatureCenter.Module/BusinessObjects/Editors/WebViewHtmlStringEditorDemo.cs";

        protected override string UsageHtml => new Section()
        {
            Content = new()
            {
                new TabGroup()
                {
                    Tabs = new()
                    {
                        new Tab("Attributes", "fas fa-code")
                        {
                            HtmlAble = MarkDownBlock.FromResourceString("BusinessObjects/Editors/WebViewHtmlStringEditorDemo.Usage.Attributes.md")
                        },
                        new Tab("ModelBuilders", "fas fa-project-diagram")
                        {
                            HtmlAble = MarkDownBlock.FromResourceString("BusinessObjects/Editors/WebViewHtmlStringEditorDemo.Usage.ModelBuilders.md")
                        },
                        new Tab("Model-Editor", "fas fa-tools")
                        {
                            HtmlAble = ImageBlock.Create("is-4by3", Resources.WebViewHtmlStringEditorDemo_Usage_ModelEditor)
                        },
                    }
                }
            }
        }.ToString();

        protected override string RemarksHtml()
            => MarkDownBlock.FromResourceString("BusinessObjects/Editors/WebViewHtmlStringEditorDemo.Remarks.md").ToString();

        protected override IEnumerable<RequiredNuget> GetRequiredModules() => new[]
        {
            new RequiredNuget("WebView"),
            new RequiredNuget("WebView", AvailablePlatform.Win),
            new RequiredNuget("WebView", AvailablePlatform.Blazor),
        };

        protected override string SupportedPlatformsHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"<div class='is-flex is-justify-content-center'>");
            sb.AppendLine("<table class='table'>");

            sb.AppendLine("<thead>");
            sb.AppendLine("<th>Platform</th>");
            sb.AppendLine("<th><abbr title='Target Framework Monikers'>TFM</abbr></th>");
            sb.AppendLine("<th><abbr title='DevExpress Version'>DxV</abbr></th>");
            sb.AppendLine("</thead>");

            sb.AppendLine("<tbody>");

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td><span class='tag'>Xenial.Framework.WebView</span></td>");
            sb.AppendLine($"<td><div class='tags'><span class='tag'>net462</span><span class='tag'>netstandard2.0</span><span class='tag'>net5.0</span></div></td>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>&gt;=</span><span class='tag is-info'>20.2.4</span></div></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>Xenial.Framework.WebView</span><span class='tag is-info'>Win</span></div></td>");
            sb.AppendLine($"<td><div class='tags'><span class='tag'>net462</span></div></td>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>&gt;=</span><span class='tag is-info'>20.2.4</span></div></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>Xenial.Framework.WebView</span><span class='tag is-info'>Blazor</span></div></td>");
            sb.AppendLine($"<td><div class='tags'><span class='tag'>netstandard2.1</span><span class='tag'>net5.0</span></div></td>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>&gt;=</span><span class='tag is-info'>20.2.4</span></div></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</tbody>");

            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        protected override IEnumerable<EditorInstallation> EditorInstallations => new[]
        {
            new EditorInstallation("XenialWebViewModule", "UseWebViewHtmlStringPropertyEditor", null),
            new EditorInstallation("XenialWebViewWindowsFormsModule", "UseWebViewHtmlStringPropertyEditorWin", AvailablePlatform.Win),
            new EditorInstallation("XenialWebViewBlazorModule", "UseWebViewHtmlStringPropertyEditorBlazor", AvailablePlatform.Blazor),
        };
    }
}
