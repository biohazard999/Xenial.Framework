using Buildalyzer;
using Buildalyzer.Environment;

using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

using ModelToCodeConverter.Engine;

using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

using Xenial.Cli.Utils;

using static Xenial.Cli.Utils.ConsoleHelper;

namespace Xenial.Cli.Commands;

public class BuildCommandSettings : BaseCommandSettings
{
    public BuildCommandSettings(string? workingDirectory) : base(workingDirectory) { }

    [Description("Project (csproj) or Solution (sln) file")]
    [CommandArgument(0, "<project>")]
    public string ProjectOrSolution { get; set; } = "";

    [Description("Doesn't execute an implicit restore during build.")]
    [CommandOption("--no-restore")]
    public bool NoRestore { get; set; }

    [Description("Doesn't execute an implicit build.")]
    [CommandOption("--no-build")]
    public bool NoBuild { get; set; }

    [Description("Specifies the msbuild log verbosity during build.")]
    [CommandOption("--build-verbosity")]
    public LoggerVerbosity? MsBuildVerbosity { get; set; }

    [Description("Specifies if msbuild should log to command line.")]
    [CommandOption("--log-build")]
    [DefaultValue(false)]
    public bool LogMSBuildCommandLine { get; set; }

    [Description("Specifies the target framework monikers. Use * for all, separate by semicolon for multiple values")]
    [CommandOption("--tfms")]
    [DefaultValue("*")]
    public string Tfms { get; set; } = "*";

    [Description("Specifies if error behavior should be strict. When set to false, it continues even if build errors are present")]
    [CommandOption("-s|--strict")]
    [DefaultValue(true)]
    public bool Strict { get; set; }

    [Description("Tries to do as little effort when building as possible. When set to false, it relaxes some conditions, but will be slower")]
    [CommandOption("--fast")]
    [DefaultValue(true)]
    public bool Fast { get; set; }

    public override ValidationResult Validate()
    {
        if (!RunAsWizard)
        {
            if (!File.Exists(ProjectOrSolution))
            {
                return ValidationResult.Error($"The project file '{ProjectOrSolution}' does not exist.");
            }
        }
        return base.Validate();
    }
}

public record BuildContext : BuildContext<BuildCommandSettings>
{

}

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

public abstract record BuildPipeline<TContext, TSettings> : Pipeline<TContext>
    where TContext : BuildContext<TSettings>
    where TSettings : BuildCommandSettings
{
}

public record BuildPipeline : BuildPipeline<BuildContext, BuildCommandSettings>
{
    public override BuildContext CreateContext() => new();
}

public class BuildCommand : BuildCommand<BuildCommandSettings, BuildPipeline, BuildContext>
{
    public BuildCommand(ILoggerFactory loggerFactory, ILogger<BuildCommand> logger) : base(loggerFactory, logger) { }

    public override string CommandName => "build";

    protected override BuildPipeline CreatePipeline() => new();
}

public interface IXenialCommand : ICommand
{
    string CommandName { get; }
}

public abstract class BuildCommand<TSettings, TPipeline, TPipelineContext> : AsyncCommand<TSettings>, IXenialCommand
    where TSettings : BuildCommandSettings
    where TPipeline : Pipeline<TPipelineContext>
    where TPipelineContext : BuildContext<TSettings>
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger<BuildCommand<TSettings, TPipeline, TPipelineContext>> Logger;

    protected BuildCommand(ILoggerFactory loggerFactory, ILogger<BuildCommand<TSettings, TPipeline, TPipelineContext>> logger)
        => (LoggerFactory, Logger) = (loggerFactory, logger);

    protected abstract TPipeline CreatePipeline();

    public static string GetWizardValue(string message)
    {
        var name = AnsiConsole.Ask<string>($"[gray bold]{message}: [/]");

        return name;
    }

    public abstract string CommandName { get; }

    protected virtual void ConfigurePipeline(TPipeline pipeline!!)
        => pipeline.Use((ctx, next) =>
        {
            using var _ = Logger.LogInformationTick($"Command `{CommandName}`");
            return next();
        })
        .Use(async (ctx, next) =>
        {
            var sw = Stopwatch.StartNew();
            try
            {
                HorizontalRule(CommandName);
                await next();
            }
            finally
            {
                AnsiConsole.WriteLine();
                HorizontalDashed($"{CommandName} [grey][[{sw.Elapsed}]][/]");
            }
        })
        .Use((ctx, next) =>
        {
            AnsiConsole.Markup("[gray bold]Working-Directory: [/]");
            WritePath(ctx.Settings.WorkingDirectory);
            AnsiConsole.Markup("[gray bold]Project-File     : [/]");
            WritePath(ctx.Settings.ProjectOrSolution);
            return next();
        })
        .Use(async (ctx, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                Logger.LogError(ex, "Fatal error in build");
                ctx.ExitCode = 1;
                ctx.Exception = ex;
            }
        })
        .Use(async (ctx, next) =>
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await next();
            }
            finally
            {
                var exitCode = ctx.ExitCode ?? 0;
                if (exitCode == 0)
                {
                    sw.Success("Total Time");
                }
                else
                {
                    sw.Fail("Total Time");
                }
            }
        })
        .Use(async (ctx, next) =>
        {
            await AnsiConsole.Status().Start("Analyzing project...", async statusContext =>
            {
                statusContext.Spinner(Spinner.Known.Star);
                ctx.StatusContext = statusContext;

                var pipeline = CreatePipeline();

                ConfigureStatusPipeline(pipeline);

                await pipeline.Execute(ctx);
            });

            if ((ctx.ExitCode ?? 0) == 0)
            {
                await next();
            }
        });

    protected static void PatchOptions(TPipelineContext ctx!!, EnvironmentOptions environmentOptions!!)
        => PatchOptions(ctx, environmentOptions.GlobalProperties);

    protected static void PatchOptions(TPipelineContext ctx!!, IProjectAnalyzer projectAnalyzer!!)
    {
        var dic = new Dictionary<string, string>();
        PatchOptions(ctx, dic);
        foreach (var pair in dic)
        {
            projectAnalyzer.SetGlobalProperty(pair.Key, pair.Value);
        }
    }

    protected static void PatchOptions(TPipelineContext ctx!!, IDictionary<string, string> globalProperties!!)
    {
        if (!ctx.Settings.Fast)
        {
            globalProperties[MsBuildProperties.SkipCompilerExecution] = "false";
            globalProperties[MsBuildProperties.BuildingProject] = "true";
            globalProperties[MsBuildProperties.AutoGenerateBindingRedirects] = "true";
        }
    }

    protected virtual void ConfigureStatusPipeline(TPipeline pipeline!!)
        => pipeline.Use(async (ctx, next) =>
        {
            var options = new AnalyzerManagerOptions
            {
                LoggerFactory = LoggerFactory
            };

            ctx.AnalyzerManager = new AnalyzerManager(options);

            ctx.ProjectAnalyzer = ctx.AnalyzerManager.GetProject(ctx.Settings.ProjectOrSolution);

            if (ctx.Settings.LogMSBuildCommandLine)
            {
                var verbosity = ctx.Settings.MsBuildVerbosity ?? ctx.Settings.Verbosity switch
                {
                    LogLevel.Trace => LoggerVerbosity.Diagnostic,
                    LogLevel.Debug => LoggerVerbosity.Detailed,
                    LogLevel.Information => LoggerVerbosity.Normal,
                    LogLevel.Warning => LoggerVerbosity.Minimal,
                    LogLevel.Error => LoggerVerbosity.Minimal,
                    LogLevel.Critical => LoggerVerbosity.Minimal,
                    LogLevel.None => LoggerVerbosity.Quiet,
                    _ => throw new ArgumentOutOfRangeException(nameof(ctx.Settings.Verbosity), ctx.Settings.Verbosity, "Can not translate from LogLevel to MSBuild logger verbosity")
                };

                ctx.ProjectAnalyzer.AddBuildLogger(new Microsoft.Build.Logging.ConsoleLogger(verbosity));
            }

            var sw = Stopwatch.StartNew();
            ctx.StatusContext!.Status("Designtime-Build...");

            var environmentOptions = new EnvironmentOptions
            {
                Restore = !ctx.Settings.NoRestore,
                DesignTime = true
            };

            PatchOptions(ctx, environmentOptions);

            if (ctx.Settings.NoBuild)
            {
                //We don't want to clean if we don't build
                environmentOptions.TargetsToBuild.Clear();
                environmentOptions.TargetsToBuild.Add("Build");
            }

            ctx.BuildResults = ctx.ProjectAnalyzer.Build(environmentOptions);

            if (ctx.BuildResults.OverallSuccess)
            {
                sw.Success("Designtime-Build");
                await next();
            }
            else
            {
                if (ctx.Settings.Strict)
                {
                    sw.Fail("Designtime-Build");
                    ctx.ExitCode = 1;
                    return;
                }
                else
                {
                    sw.Warn("Designtime-Build");
                    await next();
                }
            }
        })
        .Use(async (ctx, next) =>
        {
            if (ctx.Settings.NoBuild)
            {
                await next();
                return;
            }

            var tfms = ctx.Settings.Tfms.Contains(';', StringComparison.OrdinalIgnoreCase)
                    ? ctx.Settings.Tfms.Trim().Split(';')
                    : new[] { ctx.Settings.Tfms.Trim() };

            tfms = (tfms.Contains("*")
                ? ctx.BuildResults!.Select(m => m.TargetFramework).ToArray()
                : tfms)
                .Where(tfm => !string.IsNullOrWhiteSpace(tfm))
                .Distinct()
                .ToArray();

            AnsiConsole.MarkupLine($"[grey bold]TFMs: [/][silver]{string.Join(", ", tfms)}[/]");

            var sw = Stopwatch.StartNew();
            ctx.StatusContext!.Status("Building...");

            var environmentOptions = new EnvironmentOptions
            {
                Restore = !ctx.Settings.NoRestore,
                DesignTime = false,
                GlobalProperties =
                {
                    ["CopyLocalLockFileAssemblies"] = "true",
                    [MsBuildProperties.CopyBuildOutputToOutputDirectory] = "true"
                },
            };

            PatchOptions(ctx, environmentOptions);

            //We don't want to clean, Design Time Build should be fine
            environmentOptions.TargetsToBuild.Clear();
            environmentOptions.TargetsToBuild.Add("Build");

            ctx.BuildResults = ctx.ProjectAnalyzer!.Build(tfms, environmentOptions);

            if (ctx.BuildResults.OverallSuccess)
            {
                sw.Success("Build");
                await next();
            }
            else
            {
                if (ctx.Settings.Strict)
                {
                    sw.Fail("Build");
                    ctx.ExitCode = 1;
                }
                else
                {
                    sw.Warn("Build");
                    await next();
                }
            }
        })
        .Use(async (ctx, next) =>
        {
            ctx.StatusContext!.Status("Completed!");
            await next();
        });


    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        var pipeline = CreatePipeline();
        var pipelineContext = pipeline.CreateContext();
        pipelineContext.Settings = settings;

        ConfigurePipeline(pipeline);

        await pipeline.Execute(pipelineContext);

        return pipelineContext.ExitCode ?? 0;
    }
}
