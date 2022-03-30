using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;

using Buildalyzer;

using DevExpress.ExpressApp;

using Microsoft.CodeAnalysis;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

using Xenial.Cli.Engine;

namespace Xenial.Cli.Commands;

public record NugetToolContext
{
    public string GlobalPackagesFolder { get; set; } = "";

    public ISettings Settings { get; set; } = default!;

    public IEnumerable<PackageSource> Sources { get; set; } = Enumerable.Empty<PackageSource>();

    public IEnumerable<Lazy<INuGetResourceProvider>> Providers { get; set; } = Enumerable.Empty<Lazy<INuGetResourceProvider>>();

    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

    public SourceCacheContext Cache { get; set; } = new SourceCacheContext();

    public ILogger Logger { get; set; } = default!;
}

public record ModelContext : ModelContext<ModelCommandSettings>
{
}

public record ModelContext<TSettings> : BuildContext<TSettings>
    where TSettings : ModelCommandSettings
{
    public IAnalyzerResult? BuildResult { get; set; }
    public FileModelStore? ModelStore { get; set; }
    public Compilation? Compilation { get; private set; }
    public Project? CurrentProject { get; private set; }
    public AdhocWorkspace? Workspace { get; set; }

    public NugetToolContext? NugetToolContext { get; set; }
    public string? DesignerPackageId { get; set; }
    public NuGetVersion? DesignerPackageVersion { get; set; }
    public DownloadResourceResult? DesignerPackage { get; set; }

    public NamedPipeClientStream? DesignerStream { get; set; }

    public string? ModelEditorId { get; set; }
    public Process? ModelEditorProcess { get; set; }
    public ModelEditorRpcClient? ModelEditor { get; set; }
    public string? ModelEditorVersion { get; set; }
    public IList<FrameworkSpecificGroup> ModelEditorTools { get; } = new List<FrameworkSpecificGroup>();

    public string? ModelEditorPackageDirectory { get; set; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                ModelEditorProcess?.Kill();
            }
            finally
            {
                ModelEditorProcess?.Dispose();
                ModelEditor?.Dispose();
                DesignerStream?.Dispose();
            }
        }
        base.Dispose(disposing);
    }


    private readonly Dictionary<Project, Compilation?> compilations = new();

    public async Task<Compilation?> GetCompilationForProject(Project project)
    {
        _ = project ?? throw new ArgumentNullException(nameof(project));

        if (compilations.TryGetValue(project, out var compilation))
        {
            return compilation;
        }
        return await project.GetCompilationAsync();
    }

    public void SetCompilationForProject(Project project, Compilation? compilation)
    {
        _ = project ?? throw new ArgumentNullException(nameof(project));
        compilations[project] = compilation;
        Compilation = compilation;
        CurrentProject = project;
    }

    public void ReplaceCurrentCompilation(Compilation? compilation)
        => SetCompilationForProject(CurrentProject!, compilation);

}
