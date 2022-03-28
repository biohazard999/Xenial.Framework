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

public class BuildCommand : BuildCommand<BuildCommandSettings, BuildPipeline, BuildContext>
{
    public BuildCommand(ILoggerFactory loggerFactory, ILogger<BuildCommand> logger) : base(loggerFactory, logger) { }

    public override string CommandName => "build";

    protected override BuildPipeline CreatePipeline() => new();
}

public abstract class BuildCommand<TSettings, TPipeline, TPipelineContext> : AsyncCommand<TSettings>, IXenialCommand
    where TSettings : BuildCommandSettings
    where TPipeline : Pipeline<TPipelineContext>
    where TPipelineContext : BuildContext<TSettings>
{
    protected ILoggerFactory LoggerFactory { get; }
    protected ILogger<BuildCommand<TSettings, TPipeline, TPipelineContext>> Logger { get; }

    protected BuildCommand(ILoggerFactory loggerFactory, ILogger<BuildCommand<TSettings, TPipeline, TPipelineContext>> logger)
        => (LoggerFactory, Logger) = (loggerFactory, logger);

    protected abstract TPipeline CreatePipeline();

    public abstract string CommandName { get; }

    protected virtual void ConfigurePipeline(TPipeline pipeline)
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
                statusContext.Spinner(Spinner.Known.Ascii);
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

    protected static void PatchOptions(TPipelineContext ctx, EnvironmentOptions environmentOptions)
        => PatchOptions(ctx, environmentOptions.GlobalProperties);

    protected static void PatchOptions(TPipelineContext ctx, IProjectAnalyzer projectAnalyzer)
    {
        var dic = new Dictionary<string, string>();
        PatchOptions(ctx, dic);
        foreach (var pair in dic)
        {
            projectAnalyzer.SetGlobalProperty(pair.Key, pair.Value);
        }
    }

    protected static void PatchOptions(TPipelineContext ctx, IDictionary<string, string> globalProperties)
    {
        globalProperties[MsBuildProperties.SkipCompilerExecution] = "false";
        globalProperties[MsBuildProperties.BuildingProject] = "true";
        globalProperties[MsBuildProperties.AutoGenerateBindingRedirects] = "true";

        globalProperties[MsBuildProperties.ComputeNETCoreBuildOutputFiles] = "true";
        globalProperties[MsBuildProperties.CopyBuildOutputToOutputDirectory] = "true";
        globalProperties[MsBuildProperties.BuildProjectReferences] = "true";
        globalProperties[MsBuildProperties.SkipCopyBuildProduct] = "false";
    }

    protected virtual void ConfigureStatusPipeline(TPipeline pipeline)
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

    public override ValidationResult Validate(CommandContext context, TSettings settings)
        => settings.Validate();

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        var pipeline = CreatePipeline();
        var pipelineContext = pipeline.CreateContext();
        pipelineContext.Settings = settings;

        pipeline.Use(async (ctx, next) =>
        {
            try
            {
                await next();
            }
            finally
            {
                ctx.Dispose();
            }
        });

        ConfigurePipeline(pipeline);

        await pipeline.Execute(pipelineContext);

        return pipelineContext.ExitCode ?? 0;
    }
}
