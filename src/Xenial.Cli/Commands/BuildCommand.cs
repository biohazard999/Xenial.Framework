using Buildalyzer;
using Buildalyzer.Environment;

using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

using Xenial.Cli.Engine;
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
            PrintInfo("Working-Directory", ToPath(ctx.Settings.WorkingDirectory));
            PrintInfo("Project-File", ToPath(ctx.Settings.ProjectOrSolution));
            return next();
        })
        .Use(async (ctx, next) =>
        {
            try
            {
                await next();
            }
            catch (RestartPipelineException) { throw; }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                Logger.LogError(ex, "Fatal error in build");
                ctx.ExitCode = 1;
                ctx.Exception = ex;
            }
        })
        .UseStatusWithTimer("Designtime-Build...", "Designtime-Build", ctx => ctx.BuildResults!.OverallSuccess, _ => true, async (ctx, next) =>
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

            ctx.DesignTimeEnvironmentOptions = new EnvironmentOptions
            {
                Restore = !ctx.Settings.NoRestore,
                DesignTime = true
            };

            PatchOptions(ctx, ctx.DesignTimeEnvironmentOptions);

            //We don't want to clean
            ctx.DesignTimeEnvironmentOptions.TargetsToBuild.Clear();
            ctx.DesignTimeEnvironmentOptions.TargetsToBuild.Add("Build");

            ctx.BuildResults = ctx.ProjectAnalyzer.Build(ctx.DesignTimeEnvironmentOptions);

            await next();

        }).Use(async (ctx, next) =>
        {
            if (!ctx.BuildResults!.OverallSuccess)
            {
                AnsiConsole.MarkupLine("[red]It seams the design time build failed.[/]");
                AnsiConsole.MarkupLine("[yellow]We will clean the following directories[/]");
                AnsiConsole.MarkupLine("[red]and run the Restore target.[/]");
                AnsiConsole.WriteLine();

                var directoriesToDelete = new List<string>();

                foreach (var buildResult in ctx.BuildResults.Results)
                {
                    var targetDir = buildResult.GetProperty("TargetDir");
                    var projectDir = buildResult.GetProperty("ProjectDir");
                    var intermediateOutputPath = buildResult.GetProperty("BaseIntermediateOutputPath");
                    if (targetDir is not null && Directory.Exists(targetDir))
                    {
                        directoriesToDelete.Add(targetDir);
                    }
                    var objDirectory = Path.Combine(projectDir, intermediateOutputPath);
                    if (objDirectory is not null && Directory.Exists(objDirectory))
                    {
                        directoriesToDelete.Add(objDirectory);
                    }
                }

                foreach (var dir in directoriesToDelete)
                {
                    AnsiConsole.MarkupLine($"\t[grey strikethrough]{dir.EscapeMarkup()}[/]");
                }

                var shouldDelete = AnsiConsole.Confirm("Do you want to [red]delete[/] those directories?");
                if (shouldDelete)
                {
                    foreach (var dir in directoriesToDelete)
                    {
                        Directory.Delete(dir, true);
                    }
                }

                ctx.DesignTimeEnvironmentOptions!.Restore = true;
                ctx.NeedsDesignTimeRebuild = true;
            }

            await next();
        })
        .UseStatusWithTimer("Designtime-ReBuild...", "Designtime-ReBuild", ctx => ctx.BuildResults!.OverallSuccess, _ => false, async (ctx, next) =>
        {
            if (ctx.NeedsDesignTimeRebuild)
            {
                ctx.BuildResults = ctx.ProjectAnalyzer!.Build(ctx.DesignTimeEnvironmentOptions);
            }
            await next();
        }, ctx => !ctx.NeedsDesignTimeRebuild)
        .Use(async (ctx, next) =>
        {
            var tfms = ctx.Settings.Tfms.Contains(';', StringComparison.OrdinalIgnoreCase)
                   ? ctx.Settings.Tfms.Trim().Split(';')
                   : new[] { ctx.Settings.Tfms.Trim() };

            tfms = (tfms.Contains("*")
                ? ctx.BuildResults!.Select(m => m.TargetFramework).ToArray()
                : tfms)
                .Where(tfm => !string.IsNullOrWhiteSpace(tfm))
                .Distinct()
                .ToArray();

            PrintInfo("Available TFM's", string.Join(", ", tfms));

            ctx.BuildTfms = tfms;
            await next();
        })
        .UseStatusWithTimer("Building...", "Build", ctx => ctx.BuildResults!.OverallSuccess, _ => false, async (ctx, next) =>
        {
            if (ctx.Settings.NoBuild)
            {
                await next();
                return;
            }

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

            ctx.BuildResults = ctx.ProjectAnalyzer!.Build(ctx.BuildTfms, environmentOptions);
            await next();
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
