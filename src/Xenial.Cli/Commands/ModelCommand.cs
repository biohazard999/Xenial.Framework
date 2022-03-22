using Buildalyzer;
using Buildalyzer.Environment;

using Buildalyzer.Workspaces;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;

using ModelToCodeConverter.Engine;

using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

using Xenial.Cli.Engine;
using Xenial.Cli.Engine.Syntax;
using Xenial.Cli.Utils;
using Xenial.Framework.DevTools.X2C;

using static Xenial.Cli.Utils.ConsoleHelper;
using static SimpleExec.Command;

namespace Xenial.Cli.Commands;

public class ModelCommandSettings : BuildCommandSettings
{
    public ModelCommandSettings(string workingDirectory) : base(workingDirectory) { }

    [Description("Specifies the target framework to load the model from. Is required when the project is multi-targeted.")]
    [CommandOption("-f|--tfm")]
    public string? Tfm { get; set; }


    [Description("Specifies which namespaces to inspect. You can specify multiple by separation by semicolon.")]
    [CommandOption("-n|--namespaces")]
    public string? Namespaces { get; set; }
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
    public Compilation? Compilation { get; set; }
    public AdhocWorkspace? Workspace { get; set; }
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

    protected override void ConfigureStatusPipeline(TPipeline pipeline!!)
    {
        base.ConfigureStatusPipeline(pipeline);

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
           ctx.StatusContext!.Status = "Loading Workspace...";
           var sw = Stopwatch.StartNew();
           try
           {
               PatchOptions(ctx, ctx.ProjectAnalyzer);

               ctx.ProjectAnalyzer.SetGlobalProperty("CopyLocalLockFileAssemblies", "true");
               ctx.ProjectAnalyzer.SetGlobalProperty(MsBuildProperties.CopyBuildOutputToOutputDirectory, "true");

               ctx.StatusContext!.Status = "Preparing Workspace...";
               ctx.Workspace = CreateWorkspace(ctx.AnalyzerManager);
               ctx.BuildResult.AddToWorkspace(ctx.Workspace, false);

               foreach (var project in ctx.Workspace.CurrentSolution.Projects)
               {
                   ctx.StatusContext!.Status = "Compiling Workspace...";
                   ctx.Compilation = await project.GetCompilationAsync();
               }
               sw.Success("Load Workspace");
               await next();
           }
           catch (Exception ex)
           {
               AnsiConsole.WriteException(ex);
               Logger.LogError(ex, "Fatal error in load workspace");
               if (ctx.Settings.Strict)
               {
                   ctx.ExitCode = 1;
                   sw.Fail("Load Workspace");
               }
               else
               {
                   sw.Warn("Load Workspace");
                   await next();
               }
           }
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
               await next();
           }
           catch
           {
               if (ctx.Settings.Strict)
               {
                   sw.Fail("Load Model");
                   ctx.ExitCode = 1;
                   throw;
               }
               else
               {
                   sw.Warn("Load Model");
                   await next();
               }
           }
       })
       ;
    }

    internal static AdhocWorkspace CreateWorkspace(IAnalyzerManager manager)
    {
        var workspace = new AdhocWorkspace();
        ILogger? logger = manager.LoggerFactory?.CreateLogger<AdhocWorkspace>();
        if (logger is not null)
        {
            workspace.WorkspaceChanged += (sender, args) => logger.LogDebug($"Workspace changed: {args.Kind.ToString()}{System.Environment.NewLine}");
            workspace.WorkspaceFailed += (sender, args) => logger.LogError($"Workspace failed: {args.Diagnostic}{System.Environment.NewLine}");
        }
        return workspace;
    }

    internal enum FileState
    {
        Unchanged,
        Modified,
        Added
    }

    protected override void ConfigurePipeline(TPipeline pipeline!!)
    {
        base.ConfigurePipeline(pipeline);

        pipeline.Use(async (ctx, next) =>
        {
            var attributeSymbol = ctx.Compilation.GetTypeByMetadataName("Xenial.Framework.Layouts.DetailViewLayoutBuilderAttribute");

            if (attributeSymbol is null)
            {
                AnsiConsole.MarkupLine($"[yellow]It seams like [/][silver]{ctx.ProjectAnalyzer.ProjectFile.Name}[/] is missing a reference to Xenial.Framework");
                var addReference = AnsiConsole.Confirm($"[silver]Do you want to add it to the project?[/]");

                if (addReference)
                {
                    var wd = Path.GetDirectoryName(ctx.ProjectAnalyzer.ProjectFile.Path)!;
                    await RunAsync("dotnet.exe", $"add {ctx.ProjectAnalyzer.ProjectFile.Name} package Xenial.Framework", wd);
                    await RunAsync("dotnet.exe", $"add {ctx.ProjectAnalyzer.ProjectFile.Name} package Xenial.Framework.Generators", wd);
                    ctx.ExitCode = 1;
                    AnsiConsole.MarkupLine($"[yellow]Added packages. Please restart command to see effects.[/]");
                    await next();
                    return;
                }
            }

            Dictionary<string, (FileState state, SyntaxTree syntaxTree)> modifications = new();
            Dictionary<string, SyntaxTree> originalSyntaxTrees = new();

            List<string> removedViews = new();

            await AnsiConsole.Status().Start("Analyzing views...", async statusContext =>
            {
                statusContext.Spinner(Spinner.Known.Star);
                ctx.StatusContext = statusContext;

                foreach (var view in ctx.Application!.Views)
                {
                    var namespaces = ctx.Settings.Namespaces?.Split(';') ?? Array.Empty<string>();

                    static bool AcceptsModelView(IModelObjectView modelObjectView, string[] namespaces)
                    {
                        if (namespaces.Length == 0)
                        {
                            return true;
                        }
                        var @namespace = modelObjectView.ModelClass.TypeInfo.Type.Namespace ?? "";

                        return namespaces.Any(ns => @namespace.StartsWith(ns, StringComparison.OrdinalIgnoreCase));
                    }

                    if (view is IModelObjectView modelObjectView
                        && AcceptsModelView(modelObjectView, namespaces)
                        )
                    {
                        AnsiConsole.MarkupLine($"[grey]Analyzing: [silver][/]{view.Id}[/]");

                        var xml = X2CEngine.ConvertToXml(view);
                        var codeResult = X2CEngine.ConvertToCode(view);

                        static Location? FindFile(TPipelineContext ctx, IModelView modelView)
                        {
                            if (modelView is IModelObjectView modelObjectView)
                            {
                                var symbol = ctx.Compilation!.GetTypeByMetadataName(modelObjectView.ModelClass.Name);
                                if (symbol is null)
                                {
                                    return null;
                                }

                                if (symbol.Locations.Any())
                                {
                                    foreach (var location in symbol.Locations.Where(m => m.IsInSource))
                                    {
                                        return location;
                                    }
                                }
                            }
                            return null;
                        }

                        var location = FindFile(ctx, view);
#if DEBUG
                        HorizontalRule(view.Id);
#endif
                        if (location is not null && location.SourceTree is not null && !string.IsNullOrEmpty(location.SourceTree.FilePath))
                        {
                            var ext = Path.GetExtension(location.SourceTree.FilePath);
                            var fileName = Path.GetFileNameWithoutExtension(location.SourceTree.FilePath);
                            var dirName = Path.GetDirectoryName(location.SourceTree.FilePath);

                            var part = view switch
                            {
                                IModelDetailView _ => ".Layouts",
                                IModelListView _ => ".Columns",
                                _ => ""
                            };

                            var builderSymbol = ctx.Compilation.GetTypeByMetadataName(codeResult.FullName);
                            var newFilePath = Path.Combine(dirName ?? "", $"{fileName}{part}{ext}");
                            if (builderSymbol is null)
                            {
                                var builderSyntax = CSharpSyntaxTree.ParseText(codeResult.Code, (CSharpParseOptions)location.SourceTree.Options, path: newFilePath);
                                ctx.Compilation = ctx.Compilation.AddSyntaxTrees(builderSyntax);
                                modifications[newFilePath] = (FileState.Added, builderSyntax);
                            }
                            else
                            {
                                var (fileState, existingSyntaxTree) = modifications.TryGetValue(newFilePath, out var r) switch
                                {
                                    true => r,
                                    false => (FileState.Unchanged, builderSymbol.Locations.First(m => m.IsInSource).SourceTree!),
                                };

                                if (!originalSyntaxTrees.ContainsKey(newFilePath))
                                {
                                    originalSyntaxTrees[newFilePath] = existingSyntaxTree;
                                }

                                var semanticModel = ctx.Compilation.GetSemanticModel(existingSyntaxTree);
                                var merger = new MergeClassesSyntaxRewriter(semanticModel, codeResult);

                                var root = await existingSyntaxTree.GetRootAsync();
                                root = merger.Visit(root);
                                root = Formatter.Format(root, ctx.Workspace!);

                                var newSyntaxTree = existingSyntaxTree.WithRootAndOptions(root, existingSyntaxTree.Options);

                                modifications[existingSyntaxTree.FilePath] = (
                                    fileState == FileState.Unchanged ? FileState.Modified : fileState,
                                    newSyntaxTree
                                );

                                ctx.Compilation = ctx.Compilation.ReplaceSyntaxTree(existingSyntaxTree, newSyntaxTree);
                            }
#if DEBUG
                            HorizontalDashed(location.SourceTree.FilePath);
                            HorizontalDashed(newFilePath);

                            if (File.Exists(location.SourceTree.FilePath))
                            {
                                var source = await File.ReadAllTextAsync(location.SourceTree.FilePath);
                                PrintSource(source);
                                AnsiConsole.WriteLine();
                            }
#endif

                            var symbol = ctx.Compilation!.GetTypeByMetadataName(modelObjectView.ModelClass.Name);

                            if (symbol is not null)
                            {
                                var root = await location.SourceTree.GetRootAsync();

                                if (!originalSyntaxTrees.ContainsKey(location.SourceTree.FilePath))
                                {
                                    originalSyntaxTrees[location.SourceTree.FilePath] = location.SourceTree;
                                }

                                var semanticModel = ctx.Compilation.GetSemanticModel(location.SourceTree);

                                if (root is not null)
                                {
                                    var attributeName = view switch
                                    {
                                        IModelDetailView _ => "DetailViewLayoutBuilder",
                                        IModelListView _ => "ListViewColumnsBuilder",
                                        _ => null
                                    };

                                    if (attributeName is not null)
                                    {
                                        var newRoot = RewriteSyntaxTree(ctx, view, codeResult, root, semanticModel);

                                        var newSyntaxTree = location.SourceTree.WithRootAndOptions(newRoot, location.SourceTree.Options);

                                        modifications[location.SourceTree.FilePath] = (FileState.Modified, newSyntaxTree);

                                        ctx.Compilation = ctx.Compilation.ReplaceSyntaxTree(location.SourceTree, newSyntaxTree);
#if DEBUG
                                        PrintSource(newRoot.ToFullString());
                                        AnsiConsole.WriteLine();
#endif
                                    }
                                }
                            }
                        }
#if DEBUG
                        PrintSource(xml, "xml");
                        AnsiConsole.WriteLine();
                        PrintSource(codeResult.Code);
                        AnsiConsole.WriteLine();
#endif
                        removedViews.Add(view.Id);
                    }
                }
            });

            ////Sanity-Check. We compare again so we don't touch mulitple files
            foreach (var oldItem in originalSyntaxTrees)
            {
                if (modifications.TryGetValue(oldItem.Key, out var modification))
                {
                    if (modification.syntaxTree.IsEquivalentTo(oldItem.Value))
                    {
                        modifications.Remove(oldItem.Key);
                    }
                }
            }

            var xafmlSyntaxRewriter = new XafmlSyntaxRewriter(ctx.ModelStore, removedViews);

            var (hasModifications, xafmlFilePath) = await xafmlSyntaxRewriter.RewriteAsync();

            if (modifications.Count > 0 || hasModifications)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[grey]There are [yellow]{modifications.Count}[/] pending modifications for project [silver]{ctx.ProjectAnalyzer.ProjectFile.Name}[/][/]");
                AnsiConsole.WriteLine();

                HorizontalDashed("Changed Files");

                if (hasModifications)
                {
                    var stateString = "[[Modified]]";
                    var stateColor = "yellow";

                    AnsiConsole.MarkupLine($"[{stateColor}]{stateString.PadRight(10)}: [silver]{xafmlFilePath}[/][/]");
                }

                foreach (var modification in modifications.Where(m => m.Value.state != FileState.Unchanged))
                {
                    var (state, syntaxTree) = modification.Value;
                    var path = modification.Key;

                    var stateString = state switch
                    {
                        FileState.Modified => "[[Modified]]",
                        FileState.Added => "[[Added]]",
                        _ => "Unknown"
                    };

                    var stateColor = state switch
                    {
                        FileState.Modified => "yellow",
                        FileState.Added => "green",
                        _ => "white"
                    };

                    path = ConsoleHelper.EllipsisPath(path);

                    AnsiConsole.MarkupLine($"[{stateColor}]{stateString.PadRight(10)}: [silver]{path}[/][/]");
                }

                var adds = modifications.Where(m => m.Value.state == FileState.Added).ToList();
                var modified = modifications.Where(m => m.Value.state == FileState.Modified).ToList();

                AnsiConsole.WriteLine();
                if (AnsiConsole.Confirm($"[silver]Do you want to proceed? [yellow][[Modified]] {modified.Count}[/] [green][[Added]] {adds.Count}[/][/]"))
                {
                    if (hasModifications)
                    {
                        var stateString = "[[Modified]]";
                        var stateColor = "yellow";
                        await xafmlSyntaxRewriter.CommitAsync();
                        AnsiConsole.MarkupLine($"[{stateColor}]{stateString.PadRight(10)}: [silver]{xafmlFilePath}[/][/]");
                    }
                    foreach (var modification in modifications.Where(m => m.Value.state != FileState.Unchanged))
                    {
                        var (state, syntaxTree) = modification.Value;
                        var path = modification.Key;

                        var stateString = state switch
                        {
                            FileState.Modified => "[[Modified]]",
                            FileState.Added => "[[Added]]",
                            _ => "Unknown"
                        };

                        var stateColor = state switch
                        {
                            FileState.Modified => "yellow",
                            FileState.Added => "green",
                            _ => "white"
                        };

                        AnsiConsole.MarkupLine($"[{stateColor}]{stateString.PadRight(10)}: [silver]{path}[/][/]");
                        await File.WriteAllTextAsync(syntaxTree.FilePath, syntaxTree.ToString());
                    }
                }
            }
            await next();
        });

        static SyntaxNode RewriteSyntaxTree(TPipelineContext ctx, IModelView? view, X2CCodeResult result, SyntaxNode? root, SemanticModel semanticModel)
        {
            if (view is IModelDetailView detailView)
            {
                var methodName = result.MethodName == "BuildLayout"
                    ? null
                    : result.MethodName;

                var rewriter = new LayoutBuilderAttributeSyntaxRewriter(semanticModel, new LayoutAttributeInfo(result.ClassName)
                {
                    ViewId = result.ViewId,
                    LayoutBuilderMethod = methodName,
                });

                root = rewriter.Visit(root);
            }
            if (view is IModelListView listView)
            {
                var methodName = result.MethodName == "BuildColumns"
                    ? null
                    : result.MethodName;

                var rewriter = new ColumnsBuilderAttributeSyntaxRewriter(semanticModel, new ColumnsAttributeInfo(result.ClassName)
                {
                    ViewId = result.ViewId,
                    ColumnsBuilderMethod = methodName,
                });

                root = rewriter.Visit(root);
            }
            root = Formatter.Format(root, ctx.Workspace!);
            return root;
        }
    }

}
