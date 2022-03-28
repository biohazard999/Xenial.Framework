using Buildalyzer;

using ModelToCodeConverter.Engine;

using Spectre.Console;

using System;
using System.Linq;

namespace Xenial.Cli.Commands;


public record BuildContext<TSettings> : PipelineContext
    where TSettings : BuildCommandSettings
{
    public TSettings Settings { get; set; } = default!;

    public IAnalyzerManager? AnalyzerManager { get; set; }
    public IProjectAnalyzer? ProjectAnalyzer { get; set; }
    public IAnalyzerResults? BuildResults { get; set; }

    public StatusContext? StatusContext { get; set; }

    public int? ExitCode { get; set; }
    public Exception? Exception { get; set; }
}
