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
            sb.AppendLine("```cmd");

            var types = GetRequiredModules();

            foreach (var type in types)
            {
                sb.AppendLine($"dotnet add package {type.Nuget} --version {XenialVersion.Version}");
            }

            sb.AppendLine("```");

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
</head>

<body style='background-color: #272822; color: #bbb; font-family: sans-serif; margin: 0; padding: 0; font-size: 14px;'>
  <h1 style='text-align: center; margin-top: .5rem'>Installation</h1>
  <hr style='border: none; border-top: 1px solid #bbb;' />
  {Markdown.ToHtml(markdown, pipeline)}
<ul class='nav nav-pills mb-3' id='pills-tab' role='tablist'>
  <li class='nav-item' role='presentation'>
    <a class='nav-link active' id='pills-home-tab' data-bs-toggle='pill' href='#pills-home' role='tab' aria-controls='pills-home' aria-selected='true'>Home</a>
  </li>
  <li class='nav-item' role='presentation'>
    <a class='nav-link' id='pills-profile-tab' data-bs-toggle='pill' href='#pills-profile' role='tab' aria-controls='pills-profile' aria-selected='false'>Profile</a>
  </li>
  <li class='nav-item' role='presentation'>
    <a class='nav-link' id='pills-contact-tab' data-bs-toggle='pill' href='#pills-contact' role='tab' aria-controls='pills-contact' aria-selected='false'>Contact</a>
  </li>
</ul>
<div class='tab-content' id='pills-tabContent'>
  <div class='tab-pane fade show active' id='pills-home' role='tabpanel' aria-labelledby='pills-home-tab'>...</div>
  <div class='tab-pane fade' id='pills-profile' role='tabpanel' aria-labelledby='pills-profile-tab'>...</div>
  <div class='tab-pane fade' id='pills-contact' role='tabpanel' aria-labelledby='pills-contact-tab'>...</div>
</div>
  <script src='https://unpkg.com/clipboard@2/dist/clipboard.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/components/prism-core.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/toolbar/prism-toolbar.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/copy-to-clipboard/prism-copy-to-clipboard.min.js'></script>
  <script src='https://unpkg.com/prismjs@1.23.0/plugins/autoloader/prism-autoloader.min.js'></script>
  <script src='https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/js/bootstrap.bundle.min.js' integrity='sha384-ygbV9kiqUc6oa4msXn9868pTtWMgiQaeYH7/t7LECLbyPA2x65Kgf80OJFdroafW' crossorigin='anonymous'></script>
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
