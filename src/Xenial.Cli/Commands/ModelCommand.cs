﻿using Buildalyzer;
using Buildalyzer.Workspaces;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Microsoft.Extensions.Logging;

using ModelToCodeConverter.Engine;

using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using static Xenial.Cli.Utils.ConsoleHelper;
using Xenial.Cli.Utils;
using Xenial.Cli.Engine;

namespace Xenial.Cli.Commands;

public class ModelCommandSettings : BuildCommandSettings
{
    public ModelCommandSettings(string workingDirectory) : base(workingDirectory) { }

    [Description("Specifies the target framework to load the model from. Is required when the project is multi-targeted.")]
    [CommandOption("--tfm")]
    public string? Tfm { get; set; }
}

public record ModelContext : ModelContext<ModelCommandSettings>
{
}

public record ModelContext<TSettings> : BuildContext<TSettings>
    where TSettings : ModelCommandSettings
{
    public IAnalyzerResult? BuildResult { get; set; }
    public IModelApplication? Application { get; set; }
    public FileModelStore? ModelStore { get; set; }
}

public abstract record ModelPipeline<TContext, TSettings> : BuildPipeline<TContext, TSettings>
    where TContext : ModelContext<TSettings>
    where TSettings : ModelCommandSettings
{
}

public record ModelPipeline : BuildPipeline<ModelContext, ModelCommandSettings>
{
    public override ModelContext CreateContext() => new();
}

public class ModelCommand : ModelCommand<ModelCommandSettings, ModelPipeline, ModelContext>
{
    public ModelCommand(ILoggerFactory loggerFactory, ILogger<ModelCommand> logger) : base(loggerFactory, logger) { }

    public override string CommandName => "model";

    protected override ModelPipeline CreatePipeline() => new();
}

public abstract class ModelCommand<TSettings, TPipeline, TPipelineContext> : BuildCommand<TSettings, TPipeline, TPipelineContext>
    where TSettings : ModelCommandSettings
    where TPipeline : Pipeline<TPipelineContext>
    where TPipelineContext : ModelContext<TSettings>
{
    protected ModelCommand(ILoggerFactory loggerFactory, ILogger<ModelCommand<TSettings, TPipeline, TPipelineContext>> logger)
        : base(loggerFactory, logger) { }

    protected override void ConfigurePipeline(TPipeline pipeline)
    {
        base.ConfigurePipeline(pipeline);

        pipeline.Use(async (ctx, next) =>
        {
            if (string.IsNullOrEmpty(ctx.Settings.Tfm) && ctx.ProjectAnalyzer!.ProjectFile.IsMultiTargeted)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"\t[red]Project [silver]{ctx.ProjectAnalyzer!.ProjectFile.Name}[/] is multi targeted.[/]");
                AnsiConsole.MarkupLine($"\t[yellow]Please specify the TargetFramework using the [silver]--tfm[/] option.[/]");
                AnsiConsole.MarkupLine($"\t[yellow]Possible values: [silver]{string.Join(", ", ctx.BuildResults!.TargetFrameworks)}[/][/]");
                AnsiConsole.WriteLine();
                var sw = Stopwatch.StartNew();
                sw.Fail("TFM");
                ctx.ExitCode = 1;
                return;
            }

            await next();
        })
        .Use(async (ctx, next) =>
        {
            ctx.BuildResult = ctx.ProjectAnalyzer!.ProjectFile.IsMultiTargeted
                ? ctx.BuildResults?.FirstOrDefault(m => m.TargetFramework == ctx.Settings.Tfm)
                : ctx.BuildResults?.FirstOrDefault();

            if (ctx.BuildResult is null)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"\t[red]Can not find correct build result for [silver]{ctx.ProjectAnalyzer!.ProjectFile.Name}[/][/]");
                AnsiConsole.MarkupLine($"\t[yellow]Please specify the TargetFramework using the [silver]--tfm[/] option.[/]");
                AnsiConsole.MarkupLine($"\t[yellow]Possible values: [silver]{string.Join(", ", ctx.BuildResults!.TargetFrameworks)}[/][/]");
                AnsiConsole.WriteLine();
                var sw = Stopwatch.StartNew();
                sw.Fail("TFM");
                ctx.ExitCode = 1;
                return;
            }
            await next();
        })
        .Use(async (ctx, next) =>
        {
            ctx.StatusContext!.Status = "Loading Application Model...";

            var assemblyPath = ctx.BuildResult!.Properties["TargetPath"];
            var folder = Path.GetDirectoryName(ctx.BuildResult!.ProjectFilePath)!;
            var targetDir = Path.GetDirectoryName(assemblyPath)!;
            var loader = new StandaloneModelEditorModelLoader();

            var sw = Stopwatch.StartNew();
            try
            {
                loader.LoadModel(
                    assemblyPath,
                    folder,
                    "",
                    targetDir
                );
                ctx.Application = loader.ModelApplication;
                ctx.ModelStore = loader.FileModelStore;
                sw.Success("Load Model");
            }
            catch
            {
                sw.Fail("Load Model");
                throw;
            }
            await next();
        })
        .Use(async (ctx, next) =>
        {
            ctx.StatusContext!.Status = "Loading Workspace...";
            var workspace = ctx.ProjectAnalyzer.GetWorkspace(false);
            foreach (var project in workspace.CurrentSolution.Projects)
            {
                ctx.StatusContext!.Status = "Preparing Workspace...";
                var compilation = await project.GetCompilationAsync();

                var type = compilation!.GetTypeByMetadataName("Xenial.FeatureCenter.Module.Model.GeneratorUpdaters.FeatureCenterNavigationItemNodesUpdater");
                if (type is not null)
                {
                    foreach (var location in type.Locations)
                    {
                        if (location.SourceTree is not null)
                        {
                            var filePath = location.SourceTree.FilePath;
                        }
                    }
                }

                ctx.StatusContext!.Status = "Preparing Workspace...";
                //SymbolFinder.so()
            }

            await next();
        });
    }
}
