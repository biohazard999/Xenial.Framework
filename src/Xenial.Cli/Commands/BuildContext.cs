using Buildalyzer;
using Buildalyzer.Environment;

using Spectre.Console;

using System;
using System.Diagnostics;
using System.Linq;

using Xenial.Cli.Engine;

namespace Xenial.Cli.Commands;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
public record BuildContext<TSettings> : PipelineContext
where TSettings : BuildCommandSettings
{
    public TSettings Settings { get; set; } = default!;

    public IAnalyzerManager? AnalyzerManager { get; set; }
    public IProjectAnalyzer? ProjectAnalyzer { get; set; }
    public IAnalyzerResults? BuildResults { get; set; }

    public EnvironmentOptions? DesignTimeEnvironmentOptions { get; set; }

    public bool NeedsDesignTimeRebuild { get; set; }

    public string[] BuildTfms { get; set; } = Array.Empty<string>();

    public Exception? Exception { get; set; }

}
