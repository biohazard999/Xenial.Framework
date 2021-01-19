using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Markdig;

using static Xenial.FeatureCenter.Module.HtmlBuilders.HtmlBuilder;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [NonPersistent]
    [Appearance("Hide.DEMO_LAYOUT_GROUP", AppearanceItemType.LayoutItem, nameof(IsNotAvailableOnPlatform), Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, TargetItems = "DEMO_LAYOUT_GROUP")]
    public abstract class FeatureCenterEditorsBaseObject : FeatureCenterBaseObjectId
    {
        protected FeatureCenterEditorsBaseObject(Session session) : base(session) { }

        protected virtual IEnumerable<RequiredNuget> GetRequiredModules() => Array.Empty<RequiredNuget>();

        protected virtual string GetNotSupportedWarning(bool includeSection = false) => string.Empty;

        [WebViewHtmlStringEditor]
        [Appearance("Hide.NotSupportedHtml", AppearanceItemType.ViewItem, nameof(IsAvailableOnPlatform), Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        public string NotSupportedHtml => BuildHtml("Demo", BuildNotSupportedHtmlWarning());

        [Browsable(false)]
        public bool IsAvailableOnPlatform => GetRequiredModules().FirstOrDefault(e => e.Platform == FeatureCenterModule.CurrentPlatform) is not null;

        [Browsable(false)]
        public bool IsNotAvailableOnPlatform => !IsAvailableOnPlatform;

        private string BuildNotSupportedHtmlWarning()
        {
            if (!IsAvailableOnPlatform)
            {
                var warning = GetNotSupportedWarning();
                if (!string.IsNullOrEmpty(warning))
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("<div class='notification is-warning is-light'>");
                    sb.AppendLine($"Platform <strong>{FeatureCenterModule.CurrentPlatform}</strong> is currently <strong>NOT</strong> supported.");
                    sb.AppendLine("</div>");
                    sb.AppendLine(warning);
                    return sb.ToString();
                }
            }

            return string.Empty;
        }

        private string BuildInstallationMarkDown()
        {
            var sb = new StringBuilder();

            var types = GetRequiredModules().Where(m => !m.Platform.HasValue || (m.Platform == FeatureCenterModule.CurrentPlatform));

            var warning = BuildNotSupportedHtmlWarning();
            if (!string.IsNullOrEmpty(warning))
            {
                sb.AppendLine(warning);
            }

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

        public record GeneratorUpdaterInstallation(string GeneratorUpdater)
        {
            public string[] Comment { get; set; } = Array.Empty<string>();
        }

        public record EditorInstallation(string Module, string EditorDescriptorsFactory, AvailablePlatform? Platform = null)
        {
            public GeneratorUpdaterInstallation? GeneratorUpdater { get; set; }
        }

        protected virtual IEnumerable<EditorInstallation> EditorInstallations { get; } = Array.Empty<EditorInstallation>();

        protected virtual void AddInstallationSection(StringBuilder sb)
        {
            string GetModuleCaption(EditorInstallation installation)
                => installation.Platform switch
                {
                    null => "Common Module",
                    AvailablePlatform.Win => "Windows Forms Module",
                    AvailablePlatform.Blazor => "Blazor Module",
                    _ => throw new ArgumentOutOfRangeException()
                };

            string GetModuleName(EditorInstallation installation)
                => installation.Platform switch
                {
                    null => "MyProjectModule",
                    AvailablePlatform.Win => "MyProjectWindowsFormsModule",
                    AvailablePlatform.Blazor => "MyProjectBlazorModule",
                    _ => throw new ArgumentOutOfRangeException()
                };

            Section CreateModuleSection(EditorInstallation installation)
                 => Section.Create(GetModuleCaption(installation), CodeBlock.Create("cs", $@"public class {GetModuleName(installation)} : ModuleBase
{{
    protected override ModuleTypeList GetRequiredModuleTypesCore()
    {{
        var moduleTypes = base.GetRequiredModuleTypesCore();
        
        moduleTypes.Add(typeof({installation.Module}));
        
        return moduleTypes;
    }}
}}"));

            Section CreateEditorDescriptorSection(EditorInstallation installation)
            {
                var genUpdater = string.Empty;
                if (installation.GeneratorUpdater is not null)
                {
                    var comment = string.Join(Environment.NewLine, installation.GeneratorUpdater.Comment.Select(c => $"        //{c}"));
                    genUpdater = $@"
    public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
    {{
        base.AddGeneratorUpdaters(updaters);{(string.IsNullOrEmpty(comment) ? string.Empty : $"{Environment.NewLine}{comment}")}
        updaters.{installation.GeneratorUpdater.GeneratorUpdater}();
    }}";
                }

                return Section.Create(GetModuleCaption(installation), CodeBlock.Create("cs", $@"public class {GetModuleName(installation)} : ModuleBase
{{
    protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
    {{
        base.RegisterEditorDescriptors(editorDescriptorsFactory);
        editorDescriptorsFactory.{installation.EditorDescriptorsFactory}();
    }}{genUpdater}
}}"));
            }

            var moduleSections = EditorInstallations.Where(m => !m.Platform.HasValue || (m.Platform.HasValue && m.Platform == FeatureCenterModule.CurrentPlatform))
                .Select(CreateModuleSection)
                .ToList();

            var editorDescriptorSections = EditorInstallations.Where(m => !m.Platform.HasValue || (m.Platform.HasValue && m.Platform == FeatureCenterModule.CurrentPlatform))
                .Select(CreateEditorDescriptorSection)
                .ToList();

            var tabGroup = Section.Create(string.Empty, new TabGroup
            {
                Tabs = new()
                {
                    new("Using Modules", "fas fa-code")
                    {
                        HtmlAble = new Section()
                        {
                            Content = new(moduleSections)
                        }
                    },
                    new("Using Feature Slices", "fas fa-pizza-slice")
                    {
                        HtmlAble = new Section()
                        {
                            Content = new(editorDescriptorSections)
                        }
                    }
                }
            });

            sb.AppendLine(tabGroup.ToString());
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
        [WebViewHtmlStringEditor]
        public string Installation => BuildInstallationHtml();

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [WebViewHtmlStringEditor]
        public string Usage => BuildUsageHtml();

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [WebViewHtmlStringEditor]
        public string Remarks => BuildHtml("Remarks", RemarksHtml());

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        [WebViewHtmlStringEditor]
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
