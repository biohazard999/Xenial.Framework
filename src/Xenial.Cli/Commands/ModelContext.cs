using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;

using Buildalyzer;

using DevExpress.ExpressApp;

using Microsoft.CodeAnalysis;

using NuGet.Packaging;

using Xenial.Cli.Engine;

namespace Xenial.Cli.Commands;

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

    private readonly Dictionary<Project, Compilation?> compilations = new();

    public async Task<Compilation?> GetCompilationForProject(Project project!!)
    {
        if (compilations.TryGetValue(project, out var compilation))
        {
            return compilation;
        }
        return await project.GetCompilationAsync();
    }

    public void SetCompilationForProject(Project project!!, Compilation? compilation)
    {
        compilations[project] = compilation;
        Compilation = compilation;
        CurrentProject = project;
    }

    public void ReplaceCurrentCompilation(Compilation? compilation)
        => SetCompilationForProject(CurrentProject!, compilation);

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
}
