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
