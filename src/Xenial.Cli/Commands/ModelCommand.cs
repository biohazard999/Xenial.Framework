using Buildalyzer;
using Buildalyzer.Environment;

using Buildalyzer.Workspaces;

using DevExpress.ExpressApp;

using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;

using Microsoft.CodeAnalysis.Formatting;
using Microsoft.Extensions.Logging;

using NuGet.Configuration;
using NuGet.Frameworks;

using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;

using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

using Spectre.Console;
using Spectre.Console.Cli;

using StreamJsonRpc;

using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;

using Xenial.Cli.Engine;
using Xenial.Cli.Engine.Syntax;
using Xenial.Cli.Utils;
using Xenial.Framework;
using Xenial.Framework.DevTools.X2C;

using static SimpleExec.Command;
using static Xenial.Cli.Utils.ConsoleHelper;

namespace Xenial.Cli.Commands;

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

    protected override void ConfigurePipeline(TPipeline pipeline)
    {
        base.ConfigurePipeline(pipeline);

        pipeline.Use(async (ctx, next) =>
        {
            //TODO: Select TFM
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
       .UseStatusWithTimer("Loading Workspace...", "Load Workspace", _ => true, _ => false, async (ctx, next) =>
       {
           PatchOptions(ctx, ctx.ProjectAnalyzer);

           ctx.ProjectAnalyzer.SetGlobalProperty("CopyLocalLockFileAssemblies", "true");
           ctx.ProjectAnalyzer.SetGlobalProperty(MsBuildProperties.CopyBuildOutputToOutputDirectory, "true");
           PrintInfo("Build-References", $"{ctx.Settings.BuildReferences}");

           ctx.Workspace = CreateWorkspace(ctx.AnalyzerManager);
           ctx.BuildResult.AddToWorkspace(ctx.Workspace, ctx.Settings.BuildReferences);

           foreach (var project in ctx.Workspace.CurrentSolution.Projects)
           {
               ctx.SetCompilationForProject(project, await project.GetCompilationAsync());
           }

           var currentProject = ctx.Workspace.CurrentSolution.Projects.FirstOrDefault(m => m.FilePath == ctx.ProjectAnalyzer.ProjectFile.Path);
           if (currentProject is not null)
           {
               ctx.SetCompilationForProject(currentProject, await currentProject.GetCompilationAsync());
           }

           await next();
       })
       .UseStatus("Locating Designer...", async (ctx, next) =>
       {
           var logger = NuGet.Common.NullLogger.Instance; //TODO: Log Nuget
           var cancellationToken = CancellationToken.None;

           var folder = Path.GetDirectoryName(ctx.BuildResult!.ProjectFilePath)!;

           var settings = Settings.LoadDefaultSettings(folder);

           // Extract some data from the settings
           var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);

           var sources = ctx.Settings.DesignerNugetFeed switch
           {
               string feed => feed.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(f => new PackageSource(f, "cli")),
               _ => SettingsUtility.GetEnabledSources(settings)
           } ?? Enumerable.Empty<PackageSource>();

           var providers = Repository.Provider.GetCoreV3();

           if (sources.Any())
           {
               var firstSource = sources.First();

               PrintInfo("Nuget-Sources", firstSource.Name, "grey");

               if (sources.Count() > 1)
               {
                   foreach (var source in sources.Skip(1))
                   {
                       PrintInfo("", source.Name, "grey");
                   }
               }
           }
           ctx.NugetToolContext = new NugetToolContext
           {
               GlobalPackagesFolder = globalPackagesFolder,
               Settings = settings,
               Sources = sources,
               Providers = providers,
               Logger = logger
           };

           ctx.DesignerPackageId = "Xenial.Design";
           ctx.DesignerPackageVersion = new NuGetVersion(
               ctx.Settings.DesignerNugetPackageVersion ?? XenialVersion.Version
           );

           PrintInfo("Designer-Package", $"{ctx.DesignerPackageId} {ctx.DesignerPackageVersion}");

           await next();

       }).UseStatus("Locating Designer...", async (ctx, next) =>
       {
           var useCache = !ctx.Settings.DesignerForceNuget;
           PrintInfo("Designer-Cached", useCache.ToString(), "grey");
           if (useCache)
           {
               ctx.DesignerPackage = GlobalPackagesFolderUtility.GetPackage(new PackageIdentity(ctx.DesignerPackageId, ctx.DesignerPackageVersion), ctx.NugetToolContext.GlobalPackagesFolder);
           }

           await next();
       }).UseStatusWithProgress("Downloading Designer...", "Download Designer", ctx => ctx.DesignerPackage is not null, _ => false, async (ctx, progress, next) =>
       {
           foreach (var source in ctx.NugetToolContext!.Sources)
           {
               var task = progress.AddTask($"Fetching Nuget {source.Name.EscapeMarkup()}")
                    .IsIndeterminate(true);

               try
               {
                   var packageIdentity = new PackageIdentity(ctx.DesignerPackageId, ctx.DesignerPackageVersion);

                   var repository = new SourceRepository(source, ctx.NugetToolContext!.Providers);

                   var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

                   if (resource is null)
                   {
                       PrintInfo("", $"{source.Name} - Resource is null", "grey");
                       continue;
                   }

                   var exists = await resource.DoesPackageExistAsync(
                       ctx.DesignerPackageId,
                       ctx.DesignerPackageVersion,
                       ctx.NugetToolContext!.Cache,
                       ctx.NugetToolContext!.Logger,
                       ctx.NugetToolContext!.CancellationToken);

                   if (!exists)
                   {
                       PrintInfo("", $"{source.Name} - Not found", "grey");
                       continue;
                   }
                   else
                   {
                       PrintInfo("", $"{source.Name} - found", "green");
                   }

                   // Download the package
                   using var packageStream = new MemoryStreamWithProgress(task);

                   if (!await resource.CopyNupkgToStreamAsync(
                       ctx.DesignerPackageId,
                       ctx.DesignerPackageVersion,
                       packageStream,
                       ctx.NugetToolContext!.Cache,
                       ctx.NugetToolContext!.Logger,
                       ctx.NugetToolContext!.CancellationToken
                   ))
                   {
                       continue;
                   }

                   packageStream.Seek(0, SeekOrigin.Begin);

                   // Add it to the global package folder
                   var downloadResult = await GlobalPackagesFolderUtility.AddPackageAsync(
                       source.Source,
                       packageIdentity,
                       packageStream,
                       ctx.NugetToolContext.GlobalPackagesFolder,
                       parentId: Guid.Empty,
                       ClientPolicyContext.GetClientPolicy(ctx.NugetToolContext.Settings, ctx.NugetToolContext.Logger),
                       ctx.NugetToolContext.Logger,
                       ctx.NugetToolContext.CancellationToken
                   );

                   if (downloadResult.Status == DownloadResourceResultStatus.Available)
                   {
                       task.StopTask();
                       PrintInfo("", $"{source.Name} - fetched", "green");
                       ctx.DesignerPackage = downloadResult;
                       break;
                   }
               }
               catch (FatalProtocolException ex)
               {
                   Logger.LogWarning(ex, "Fatal Nuget Error");

                   if (ex.InnerException is HttpRequestException httpRequestEx)
                   {
                       if (httpRequestEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                       {
                           PrintInfo("", $"{source.Name}: Authorized Feeds are not supported yet", "olive");
                           continue;
                       }

                       throw;
                   }
               }
               finally
               {
                   task.StopTask();
               }
           }

           await next();
       }, skipWhen: ctx => ctx.DesignerPackage is not null)
       .Use(async (ctx, next) =>
       {
           var tools = await ctx.DesignerPackage.PackageReader.GetToolItemsAsync(ctx.NugetToolContext.CancellationToken);
           ctx.ModelEditorVersion = ctx.DesignerPackageVersion.ToString();
           ctx.ModelEditorTools.AddRange(tools.OfType<FrameworkSpecificGroup>());

           var versionStr = ctx.DesignerPackageVersion.ToString();
           ctx.ModelEditorPackageDirectory = Path.Combine(ctx.NugetToolContext.GlobalPackagesFolder, ctx.DesignerPackageId, versionStr);

           await next();
       })
       .UseStatus("Launching Designer...", async (ctx, next) =>
       {
           var tfm = ctx.BuildResult!.TargetFramework;
           var launcher = tfm switch
           {
               var s when s.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) => "net6.0-windows",
               var s when s.StartsWith("net5.", StringComparison.OrdinalIgnoreCase) => "net5.0-windows",
               var s when s.StartsWith("net6.", StringComparison.OrdinalIgnoreCase) => "net6.0-windows",
               var s when s.StartsWith("net7.", StringComparison.OrdinalIgnoreCase) => "net7.0-windows",
               var t => t,
           };

           var processPath = await FindProcessPath(launcher, ctx);

           ctx.ModelEditorId = Guid.NewGuid().ToString();

           if (!File.Exists(processPath))
           {
               Stopwatch.StartNew().Fail($"Designer does not exist {processPath}");
               Logger.LogError($"Designer does not exist {processPath}");
           }
           else
           {
               var sw = Stopwatch.StartNew();
               try
               {
                   var info = new ProcessStartInfo
                   {
                       FileName = processPath,
                       ArgumentList =
                   {
                       ctx.ModelEditorId,
#pragma warning disable CA1308
                       ctx.Settings.DesignerDebug.ToString().ToLowerInvariant()
#pragma warning restore CA1308,
                   }
                   };

                   ctx.ModelEditorProcess = Process.Start(info);

                   sw.Success("Launching Designer");

                   await next();
               }
               catch (Exception ex)
               {
                   sw.Fail("Launching Designer");
                   Logger.LogError(ex, "Launching Designer");
                   throw;
               }
           }
       })
       .UseStatus("Connecting to Designer...", async (ctx, next) =>
       {
           var sw = Stopwatch.StartNew();
           try
           {
               ctx.DesignerStream = new NamedPipeClientStream(".", ctx.ModelEditorId, PipeDirection.InOut, PipeOptions.Asynchronous);

               if (ctx.Settings.DesignerDebug)
               {
                   await ctx.DesignerStream.ConnectAsync();
               }
               else
               {
                   await ctx.DesignerStream.ConnectAsync(ctx.Settings.DesignerConnectionTimeout);
               }

               sw.Success("Connecting to Designer");
               await next();
           }
           catch (Exception ex)
           {
               sw.Fail("Connecting to Designer");
               Logger.LogError(ex, "Connecting to Designer");
               throw;
           }
       })
       .UseStatus("Connecting Designer RPC...", async (ctx, next) =>
       {
           var sw = Stopwatch.StartNew();
           try
           {
               var attached = JsonRpc.Attach(ctx.DesignerStream!);

               ctx.ModelEditor = new ModelEditorRpcClient(attached);

               var ping = await ctx.ModelEditor.Ping();

               sw.Success($"Connecting Designer RPC: [green]{ping.EscapeMarkup()}[/]");

               await next();
           }
           catch (Exception ex)
           {
               sw.Fail($"Connecting Designer RPC");
               Logger.LogError(ex, "Connecting Designer RPC");
               throw;
           }
       })
       .UseStatus("Loading Application Model...", async (ctx, next) =>
       {
           var sw = Stopwatch.StartNew();
           try
           {
               var assemblyPath = ctx.BuildResult!.Properties["TargetPath"];
               var folder = Path.GetDirectoryName(ctx.BuildResult!.ProjectFilePath)!;
               var targetDir = Path.GetDirectoryName(assemblyPath)!;

               var numberOfViews = await ctx.ModelEditor.LoadModel(assemblyPath,
                   folder,
                   "",
                   targetDir);

               sw.Success($"Load Model: {numberOfViews} views");
               await next();
           }
           catch (Exception ex)
           {
               Logger.LogError(ex, "Load Model");
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
       .Use(async (ctx, next) =>
        {
            var result = await ctx.ModelEditor!.Pong();

            AnsiConsole.WriteLine(result);

            await next();
        })
       .Use(async (ctx, next) =>
        {
            var attributeSymbol = ctx.Compilation.GetTypeByMetadataName(typeof(XenialModuleBase).FullName!);

            if (attributeSymbol is null)
            {
                AnsiConsole.MarkupLine($"[yellow]It seems like [/][silver]{ctx.ProjectAnalyzer.ProjectFile.Name}[/] is missing a reference to Xenial.Framework");
                var addReference = AnsiConsole.Confirm($"[silver]Do you want to add it to the project?[/]");

                if (addReference)
                {
                    var wd = Path.GetDirectoryName(ctx.ProjectAnalyzer.ProjectFile.Path)!;
                    await RunAsync("dotnet.exe", $"add {ctx.ProjectAnalyzer.ProjectFile.Name} package Xenial.Framework", wd);
                    await RunAsync("dotnet.exe", $"add {ctx.ProjectAnalyzer.ProjectFile.Name} package Xenial.Framework.Generators", wd);
                    AnsiConsole.MarkupLine($"[yellow]Added packages. Restart command...[/]");
                    throw new RestartPipelineException("Installed packages. Restarting command");
                }
            }
            await next();
        })
        .UseStatus("Analyzing views...", async (ctx, next) =>
        {
            Dictionary<string, (FileState state, SyntaxTree syntaxTree)> modifications = new();
            Dictionary<string, SyntaxTree> originalSyntaxTrees = new();

            List<string> removedViews = new();

            var namespacesFilter = ctx.Settings.Namespaces?.Split(';') ?? Array.Empty<string>();
            var viewsFilter = ctx.Settings.Views?.Split(';') ?? Array.Empty<string>();

            var views = await ctx.ModelEditor.GetViewIds(namespacesFilter);

            views = viewsFilter.Length > 0
                ? views.Where(viewId => viewsFilter.Contains(viewId)).ToList()
                : views;

            foreach (var viewId in views)
            {
                AnsiConsole.MarkupLine($"[grey]Analyzing: [silver][/]{viewId}[/]");

                var xml = await ctx.ModelEditor.GetViewAsXml(viewId);
                var viewType = await ctx.ModelEditor.GetViewType(viewId);
                var modelClass = await ctx.ModelEditor.GetModelClass(viewId);

                //TODO: move to Design later once we have customizable X2CCode output
                var codeResult = X2CEngine.ConvertToCode(xml);

                var location = FindFile(ctx, modelClass);
#if DEBUG
                HorizontalRule(viewId);
#endif
                if (location is not null && location.SourceTree is not null && !string.IsNullOrEmpty(location.SourceTree.FilePath))
                {
                    var ext = Path.GetExtension(location.SourceTree.FilePath);
                    var fileName = Path.GetFileNameWithoutExtension(location.SourceTree.FilePath);
                    var dirName = Path.GetDirectoryName(location.SourceTree.FilePath);

                    var part = viewType switch
                    {
                        ViewType.DetailView => ".Layouts",
                        ViewType.ListView => ".Columns",
                        _ => ""
                    };

                    var builderSymbol = ctx.Compilation!.GetTypeByMetadataName(codeResult.FullName);
                    var newFilePath = Path.Combine(dirName ?? "", $"{fileName}{part}{ext}");
                    if (builderSymbol is null)
                    {
                        var builderSyntax = CSharpSyntaxTree.ParseText(codeResult.Code, (CSharpParseOptions)location.SourceTree.Options, path: newFilePath);
                        ctx.ReplaceCurrentCompilation(ctx.Compilation.AddSyntaxTrees(builderSyntax));
                        modifications[newFilePath] = (FileState.Added, builderSyntax);
                    }
                    else
                    {
                        await ReplaceSyntaxTree(ctx, modifications, originalSyntaxTrees, builderSymbol, newFilePath, codeResult);
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
                    var symbol = ctx.Compilation!.GetTypeByMetadataName(modelClass!);

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
                            var attributeName = viewType switch
                            {
                                ViewType.DetailView => "DetailViewLayoutBuilder",
                                ViewType.ListView => "ListViewColumnsBuilder",
                                _ => null
                            };

                            if (attributeName is not null)
                            {
                                var newRoot = RewriteSyntaxTree(ctx, viewType, codeResult, root, semanticModel);

                                var newSyntaxTree = location.SourceTree.WithRootAndOptions(newRoot, location.SourceTree.Options);

                                modifications[location.SourceTree.FilePath] = (FileState.Modified, newSyntaxTree);

                                ctx.ReplaceCurrentCompilation(ctx.Compilation.ReplaceSyntaxTree(location.SourceTree, newSyntaxTree));
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
                removedViews.Add(viewId);
            }

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

            var (hasXamlModifications, xafmlFilePath) = await xafmlSyntaxRewriter.RewriteAsync();

            var csProjSyntaxRewriter = new CsProjSyntaxRewriter(ctx.ProjectAnalyzer!, ctx.BuildResult!, modifications);

            var (hasCsProjModifications, csprojFilePath) = await csProjSyntaxRewriter.RewriteAsync();

            if (modifications.Count > 0 || hasXamlModifications)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[grey]There are [yellow]{modifications.Count + (hasXamlModifications ? 1 : 0)}[/] pending modifications for project [silver]{ctx.ProjectAnalyzer.ProjectFile.Name}[/][/]");
                AnsiConsole.WriteLine();

                HorizontalDashed("Changed Files");

                if (hasXamlModifications)
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
                    if (hasXamlModifications)
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
    }

    private static async Task ReplaceSyntaxTree(TPipelineContext ctx, Dictionary<string, (FileState state, SyntaxTree syntaxTree)> modifications, Dictionary<string, SyntaxTree> originalSyntaxTrees, INamedTypeSymbol builderSymbol, string newFilePath, X2CCodeResult codeResult)
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

        try
        {
            var semanticModel = ctx.Compilation!.GetSemanticModel(existingSyntaxTree);
            var merger = new MergeClassesSyntaxRewriter(semanticModel, codeResult);

            var root = await existingSyntaxTree.GetRootAsync();
            root = merger.Visit(root);
            root = Formatter.Format(root, ctx.Workspace!);

            var newSyntaxTree = existingSyntaxTree.WithRootAndOptions(root, existingSyntaxTree.Options);

            modifications[existingSyntaxTree.FilePath] = (
                fileState == FileState.Unchanged ? FileState.Modified : fileState,
                newSyntaxTree
            );


            ctx.ReplaceCurrentCompilation(ctx.Compilation.ReplaceSyntaxTree(existingSyntaxTree, newSyntaxTree));
        }
        catch (ArgumentException)
        {
            var found = false;
            foreach (var project in ctx.Workspace!.CurrentSolution.Projects)
            {
                ctx.Workspace!.CurrentSolution.GetDocumentIdsWithFilePath(existingSyntaxTree.FilePath);
                var file = project.Documents.FirstOrDefault(m => m.FilePath == existingSyntaxTree.FilePath);
                if (file is not null)
                {
                    ctx.SetCompilationForProject(project, await ctx.GetCompilationForProject(project));
                    found = true;
                    break;
                }
            }
            if (found)
            {
                await ReplaceSyntaxTree(ctx, modifications, originalSyntaxTrees, builderSymbol, newFilePath, codeResult);
            }
            else
            {
                throw;
            }
        }
    }

    private static SyntaxNode RewriteSyntaxTree(TPipelineContext ctx, ViewType viewType, X2CCodeResult result, SyntaxNode? root, SemanticModel semanticModel)
    {
        if (viewType == ViewType.DetailView)
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
        if (viewType == ViewType.ListView)
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
        root = Formatter.Format(root!, ctx.Workspace!);
        return root;
    }

    private static Location? FindFile(TPipelineContext ctx, string? modelClass)
    {
        if (!string.IsNullOrEmpty(modelClass))
        {
            var symbol = ctx.Compilation!.GetTypeByMetadataName(modelClass);
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

    private static Task<string> FindProcessPath(string launcher, TPipelineContext ctx)
    {
        if (ctx.Settings.DesignerLaunchFromNuget)
        {
            if (ctx.ModelEditorTools is not null && ctx.ModelEditorTools.Count > 0)
            {
                var x = new NuGetFramework("net462");

                var tfms = ConvertToTfms(ctx.ModelEditorTools);

                var tools = FindGroup(tfms, launcher);
                if (tools is not null && !string.IsNullOrEmpty(ctx.ModelEditorPackageDirectory))
                {
                    foreach (var tool in tools.Items)
                    {
                        var path = Path.Combine(ctx.ModelEditorPackageDirectory, tool.Replace('/', Path.DirectorySeparatorChar));
                        if (File.Exists(path))
                        {
                            var fileName = Path.GetFileName(path);
                            if (fileName == "Xenial.Design.exe")
                            {
                                return Task.FromResult(path);
                            }
                        }
                    }
                }
            }
        }

#if DEBUG
        const string configuration = "Debug";
#else
        const string configuration = "Release";
#endif
        var processPath = $@"..\..\..\..\Xenial.Design\bin\{configuration}\{launcher}\Xenial.Design.exe";

        return Task.FromResult(processPath!);
    }

    private static Dictionary<string, FrameworkSpecificGroup> ConvertToTfms(IEnumerable<FrameworkSpecificGroup> groups)
    {
        var result = new Dictionary<string, FrameworkSpecificGroup>();

        foreach (var group in groups)
        {
            if (group.TargetFramework.Framework == ".NETFramework")
            {
                var tfmVersion = group.TargetFramework.Version.ToString().Replace(".", "", StringComparison.OrdinalIgnoreCase).TrimEnd('0');
                var tfm = $"net{tfmVersion}";
                result[tfm] = group;
            }
            else
            {
                result[group.TargetFramework.ToString()] = group;
            }
        }

        return result;
    }

    private static FrameworkSpecificGroup? FindGroup(Dictionary<string, FrameworkSpecificGroup> groups, string launcher)
    {
        if (groups.TryGetValue(launcher, out var group))
        {
            return group;
        }
        foreach (var pair in groups)
        {
            if (pair.Key.StartsWith(launcher, StringComparison.OrdinalIgnoreCase))
            {
                return pair.Value;
            }
        }
        return null;
    }
}
