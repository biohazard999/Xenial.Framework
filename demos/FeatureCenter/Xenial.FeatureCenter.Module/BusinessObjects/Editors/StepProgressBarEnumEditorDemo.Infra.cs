using System;
using System.Collections.Generic;
using System.Text;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public partial class StepProgressBarEnumEditorDemo
    {
        protected override string DemoCodeFileName => "demos/FeatureCenter/Xenial.FeatureCenter.Module/BusinessObjects/Editors/StepProgressBarEnumEditorDemo.cs";

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
                            HtmlAble = MarkDownBlock.FromResourceString("BusinessObjects/Editors/StepProgressBarEnumEditorDemo.Usage.Attributes.md")
                        },
                        new Tab("ModelBuilders", "fas fa-project-diagram")
                        {
                            HtmlAble = MarkDownBlock.FromResourceString("BusinessObjects/Editors/StepProgressBarEnumEditorDemo.Usage.ModelBuilders.md")
                        },
                        new Tab("Model-Editor", "fas fa-tools")
                        {
                            HtmlAble = ImageBlock.Create("is-4by3", Resources.StepProgressBarEnumEditorDemo_Usage_ModelEditor)
                        },
                    }
                }
            }
        }.ToString();

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
            sb.AppendLine($"<td><span class='tag'>Xenial.Framework.StepProgressEditors</span></td>");
            sb.AppendLine($"<td><div class='tags'><span class='tag'>net462</span><span class='tag'>netstandard2.0</span><span class='tag'>net5.0</span></div></td>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>&gt;=</span><span class='tag is-info'>20.2.4</span></div></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>Xenial.Framework.StepProgressEditors</span><span class='tag is-info'>Win</span></div></td>");
            sb.AppendLine($"<td><div class='tags'><span class='tag'>net462</span></div></td>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>&gt;=</span><span class='tag is-info'>20.2.4</span></div></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td><div class='tags has-addons'><span class='tag'>Xenial.Framework.StepProgressEditors</span><span class='tag is-danger is-light'>Blazor</span></div></td>");
            sb.AppendLine($"<td><abbr title='Comming soon'>-</abbr></td>");
            sb.AppendLine($"<td><abbr title='Comming soon'>-</abbr></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</tbody>");

            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div class='section'>");
            sb.AppendLine("<div class='notification is-warning is-light'>");
            sb.AppendLine("<strong>Blazor</strong> support will be implemented once DevExpress provides a similar control.");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        protected override string RemarksHtml()
            => MarkDownBlock.FromResourceString("BusinessObjects/Editors/StepProgressBarEnumEditorDemo.Remarks.md").ToString();

        protected override IEnumerable<RequiredNuget> GetRequiredModules() => new[]
        {
            new RequiredNuget("StepProgressEditors"),
            new RequiredNuget("StepProgressEditors", AvailablePlatform.Win),
        };

        protected override IEnumerable<EditorInstallation> EditorInstallations => new[]
        {
            new EditorInstallation("XenialStepProgressEditorsModule", "UseStepProgressEnumPropertyEditors", null),
            new EditorInstallation("XenialStepProgressEditorsWindowsFormsModule", "UseStepProgressEnumPropertyEditorsWin", AvailablePlatform.Win)
            {
                GeneratorUpdater = new("UseStepProgressEnumPropertyEditors")
                {
                    Comment = new []
                    {
                        "This is optional.",
                        "You need this only for nullable enumeration properties in combination with the",
                        "EditorAliasAttribute, StepProgressEnumEditorAttribute or ModelBuilders",
                        "when using DevExpress version <= 20.2.4",
                        "See: https://supportcenter.devexpress.com/ticket/details/t962834/registering-an-editor-alias-for-nullable-types for more information"
                    }
                }
            }
        };
    }
}
