using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Markdig;

namespace Xenial.FeatureCenter.Module.BusinessObjects
{
    [NonPersistent]
    public abstract class FeatureCenterBaseObjectId : FeatureCenterBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XenialLicenseBaseObjectId"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public FeatureCenterBaseObjectId(Session session) : base(session) { }

        [Key(AutoGenerate = true)]
        [Persistent("Id")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Needed by XPO")]
        private int id = -1;
        [PersistentAlias(nameof(id))]
        [Browsable(false)]
        public int Id => id;
    }

    [NonPersistent]
    public abstract class FeatureCenterDemoBaseObjectId : FeatureCenterBaseObjectId
    {
        protected FeatureCenterDemoBaseObjectId(Session session) : base(session) { }

        protected virtual IEnumerable<RequiredNuget> GetRequiredModules() => Array.Empty<RequiredNuget>();

        private string BuildInstallationMarkDown()
        {
            var sb = new StringBuilder();

            var types = GetRequiredModules();

            var installers = new[]
            {
                ("dotnet-cli", ".NET CLI", "cmd", types.Select(t => $"dotnet add package {t.Nuget} --version {XenialVersion.Version}")),
                ("package-reference", "PackageReference", "xml", types.Select(t => $"<PackageReference Include=\"{t.Nuget}\" Version=\"{XenialVersion.Version}\" />")),
                ("package-manager", "Package Manager", "powershell", types.Select(t => $"Install-Package {t.Nuget} -Version {XenialVersion.Version}")),
                ("paket-cli", "Paket CLI", "cmd", types.Select(t => $"paket add {t.Nuget} --version {XenialVersion.Version}")),
            };

            sb.AppendLine("<ul class='nav nav-pills nav-fill' role='tablist'>");

            foreach (var (id, caption, _, _) in installers)
            {
                var active = installers.First().Item1 == id ? "active" : string.Empty;
                sb.AppendLine("<li class='nav-item'>");
                sb.AppendLine($"<a class='nav-link {active}' data-bs-toggle='pill' href='#{id}' role='tab'>{caption}</a>");
                sb.AppendLine("</li>");
            }

            sb.AppendLine("</ul>");
            sb.AppendLine("<div class='tab-content'>");

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            foreach (var (id, _, type, content) in installers)
            {
                var active = installers.First().Item1 == id ? "show active" : string.Empty;
                sb.AppendLine($"<div class='tab-pane fade {active}' id='{id}'>");

                var subSb = new StringBuilder();
                subSb.AppendLine($"```{type}");
                subSb.AppendLine(string.Join(Environment.NewLine, content));
                subSb.AppendLine("```");

                sb.AppendLine(Markdown.ToHtml(subSb.ToString(), pipeline));
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private string BuildInstallationHtml()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdown = BuildInstallationMarkDown();

            var html = $@"<!doctype html>

<html lang=""en"">
<head>
  <meta charset=""utf-8"">

  <title>Xenial.FeatureCenter</title>
  <meta name=""description"" content=""Xenial.FeatureCenter"" >
  <meta name=""author"" content=""Manuel Grundner"">
  <link href='https://unpkg.com/prismjs@1.23.0/themes/prism-okaidia.css' rel='stylesheet' />
  <link href='https://unpkg.com/prismjs@1.23.0/plugins/toolbar/prism-toolbar.css' rel='stylesheet' />
  <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1' crossorigin='anonymous'>
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

.base-container {{
  max-width: 95%;
  margin-left: auto;
  margin-right: auto;
}}

</style>
</head>

<body style='background-color: #272822; color: #bbb; font-family: sans-serif; margin: 0; padding: 0; font-size: 14px;'>
  <h1 style='text-align: center; margin-top: .5rem'>Installation</h1>
  <hr style='border: none; border-top: 1px solid #bbb;' />
  <div class='base-container'>
    { Markdown.ToHtml(markdown, pipeline)}
  </div>
  <script src='https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/js/bootstrap.bundle.min.js' integrity='sha384-ygbV9kiqUc6oa4msXn9868pTtWMgiQaeYH7/t7LECLbyPA2x65Kgf80OJFdroafW' crossorigin='anonymous'></script>
  <script src='https://unpkg.com/clipboard@2/dist/clipboard.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-core.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/toolbar/prism-toolbar.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/copy-to-clipboard/prism-copy-to-clipboard.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/autoloader/prism-autoloader.min.js'></script>
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
        public string InstallationHtml => BuildInstallationHtml();
    }

    public record RequiredNuget(string ModuleName, AvailablePlatform? Platform = null)
    {
        private string nugetPostFix = Platform.HasValue ? $".{Platform.Value}" : string.Empty;
        public string Nuget => $"Xenial.Framework.{ModuleName}{nugetPostFix}";
    }

    public enum AvailablePlatform
    {
        Win,
        Blazor
    }
}
