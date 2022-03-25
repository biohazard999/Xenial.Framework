using Buildalyzer;
using Buildalyzer.Environment;

using Buildalyzer.Workspaces;

using DevExpress.ExpressApp;

using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;

using Microsoft.CodeAnalysis.Formatting;
using Microsoft.Extensions.Logging;

using ModelToCodeConverter.Engine;

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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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

public class ModelCommandSettings : BuildCommandSettings
{
    public ModelCommandSettings(string workingDirectory) : base(workingDirectory) { }

    [Description("Specifies the target framework to load the model from. Is required when the project is multi-targeted.")]
    [CommandOption("-f|--tfm")]
    public string? Tfm { get; set; }


    [Description("Specifies which namespaces to inspect. You can specify multiple by separation by semicolon.")]
    [CommandOption("-n|--namespaces")]
    public string? Namespaces { get; set; }

    [Description("Specifies the nuget feed to restore the designer package. By default it will use the project package sources. You can specify multiple by separation by semicolon.")]
    [CommandOption("--designer-nuget-feed")]
    public string? DesignerNugetFeed { get; set; }

    [Description("Specifies the package version to restore the designer package. By default it will autodetect the correct version.")]
    [CommandOption("--designer-nuget-package-version")]
    public string? DesignerNugetPackageVersion { get; set; }

    [Description("Launches the design process with an attached debugger.")]
    [CommandOption("--designer-debug")]
    [DefaultValue(false)]
    public bool DesignerDebug { get; set; }

    [Description("Specifies if the project references should be built.")]
    [CommandOption("-r|--references")]
    [DefaultValue(true)]
    public bool BuildReferences { get; set; }

    [Description("For internal use only.")]
    [CommandOption("--launch-from-nuget")]
    [DefaultValue(true)]
    public bool LaunchFromNuget { get; set; }

    [Description("Specifies the connection timeout for the design process communitcation in milliseconds. Will be unlimited when debugger is attached.")]
    [CommandOption("--designer-connection-timeout")]
    [DefaultValue(10000)]
    public int DesignerConnectionTimeout { get; set; }
}

public record ModelContext : ModelContext<ModelCommandSettings>
{
}

public record ModelContext<TSettings> : BuildContext<TSettings>
    where TSettings : ModelCommandSettings
{
    public IAnalyzerResult? BuildResult { get; set; }
    public FileModelStore? ModelStore { get; set; }
    public Compilation? Compilation { get; set; }
    public AdhocWorkspace? Workspace { get; set; }

    public NamedPipeClientStream? DesignerStream { get; set; }

    public string? ModelEditorId { get; set; }
    public Process? ModelEditorProcess { get; set; }
    public ModelEditor? ModelEditor { get; set; }
    public string? ModelEditorVersion { get; set; }
    public IList<FrameworkSpecificGroup>? ModelEditorTools { get; set; }

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
               ctx.BuildResult.AddToWorkspace(ctx.Workspace, ctx.Settings.BuildReferences);

               foreach (var project in ctx.Workspace.CurrentSolution.Projects)
               {
                   ctx.StatusContext!.Status = "Compiling Workspace...";
                   ctx.Compilation = await project.GetCompilationAsync();
               }

               var currentProject = ctx.Workspace.CurrentSolution.Projects.FirstOrDefault(m => m.FilePath == ctx.ProjectAnalyzer.ProjectFile.Path);
               if (currentProject is not null)
               {
                   ctx.Compilation = await currentProject.GetCompilationAsync();
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
           ctx.StatusContext!.Status = "Fetching Designer...";

           var sw = Stopwatch.StartNew();
           try
           {
               var logger = NuGet.Common.NullLogger.Instance;
               var cancellationToken = CancellationToken.None;

               var folder = Path.GetDirectoryName(ctx.BuildResult!.ProjectFilePath)!;

               var settings = Settings.LoadDefaultSettings(folder);

               // Extract some data from the settings
               var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);

               var sources = ctx.Settings.DesignerNugetFeed switch
               {
                   string feed => feed.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(f => new PackageSource(f)),
                   _ => SettingsUtility.GetEnabledSources(settings)
               };

               var cache = new SourceCacheContext();
               var providers = Repository.Provider.GetCoreV3();
               foreach (var source in sources)
               {
                   try
                   {
                       var repository = new SourceRepository(source, providers);

                       var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

                       const string packageId = "Xenial.Design";

                       var version = new NuGetVersion(
                           ctx.Settings.DesignerNugetPackageVersion ?? XenialVersion.Version
                       );

                       ctx.StatusContext!.Status = $"Fetching Nugets {packageId.EscapeMarkup()}.{version.ToString().EscapeMarkup()} from {source.ToString().EscapeMarkup()}...";

                       static async Task<DownloadResourceResult?> FindPackage(
                           PackageSource source,
                           FindPackageByIdResource resource,
                           string globalPackagesFolder,
                           SourceCacheContext cache,
                           NuGet.Common.ILogger logger,
                           ISettings? settings,
                           string packageId,
                           NuGetVersion version,
                           CancellationToken cancellationToken)
                       {
                           var package = GlobalPackagesFolderUtility.GetPackage(new PackageIdentity(packageId, version), globalPackagesFolder);
                           if (package is null)
                           {
                               if (await resource.DoesPackageExistAsync(packageId, version, cache, logger, cancellationToken))
                               {
                                   // Download the package
                                   using var packageStream = new MemoryStream();
                                   await resource.CopyNupkgToStreamAsync(
                                       packageId,
                                       version,
                                       packageStream,
                                       cache,
                                       logger,
                                       cancellationToken);

                                   packageStream.Seek(0, SeekOrigin.Begin);

                                   // Add it to the global package folder
                                   var downloadResult = await GlobalPackagesFolderUtility.AddPackageAsync(
                                       source.Source,
                                       new PackageIdentity(packageId, version),
                                       packageStream,
                                       globalPackagesFolder,
                                       parentId: Guid.Empty,
                                       ClientPolicyContext.GetClientPolicy(settings, logger),
                                       logger,
                                       cancellationToken);

                                   if (downloadResult.Status == DownloadResourceResultStatus.Available)
                                   {
                                       return downloadResult;
                                   }
                               }
                           }
                           return package;
                       }


                       var package = await FindPackage(source, resource, globalPackagesFolder, cache, logger, settings, packageId, version, cancellationToken);
                       if (package is not null)
                       {
                           var tools = await package.PackageReader.GetToolItemsAsync(cancellationToken);
                           ctx.ModelEditorVersion = version.ToString();
                           ctx.ModelEditorTools = tools.OfType<FrameworkSpecificGroup>().ToList();

                           var versionStr = version.ToString();
                           ctx.ModelEditorPackageDirectory = Path.Combine(globalPackagesFolder, packageId, versionStr);

                           break;
                       }

                   }
                   catch (FatalProtocolException ex)
                   {
                       if (ex.InnerException is HttpRequestException httpRequestEx)
                       {
                           if (httpRequestEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                           {
                               AnsiConsole.WriteLine($"[grey][[INFO]]    Authorized Feeds are not supported yet: {source.Source.EscapeMarkup()}[/]");
                               AnsiConsole.WriteException(ex);
                           }
                       }
                       Logger.LogWarning(ex, "Fatal Nuget Error");
                   }
               }

               sw.Success($"Fetching Designer: {ctx.ModelEditorVersion}");
               await next();
           }
           catch (Exception ex)
           {
               sw.Fail("Fetching Designer");
               Logger.LogError(ex, "Fetching Designer");
               throw;
           }

       })
       .Use(async (ctx, next) =>
       {
           ctx.StatusContext!.Status = "Launching Designer...";

           var tfm = ctx.BuildResult!.TargetFramework;
           var launcher = tfm switch
           {
               var s when s.StartsWith("netstandard") => "net6.0-windows",
               var s when s.StartsWith("net5.") => "net5.0-windows",
               var s when s.StartsWith("net6.") => "net6.0-windows",
               var s when s.StartsWith("net7.") => "net7.0-windows",
               var t => t,
           };

           static async Task<string> FindProcessPath(string launcher, TPipelineContext ctx)
           {
               if (ctx.Settings.LaunchFromNuget)
               {
                   if (ctx.ModelEditorTools is not null && ctx.ModelEditorTools.Count > 0)
                   {
                       var x = new NuGetFramework("net462");

                       static Dictionary<string, FrameworkSpecificGroup> ConvertToTfms(IEnumerable<FrameworkSpecificGroup> groups)
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

                       static FrameworkSpecificGroup? FindGroup(Dictionary<string, FrameworkSpecificGroup> groups, string launcher)
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
                                       return path;
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

               return processPath!;
           }

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
       .Use(async (ctx, next) =>
       {
           ctx.StatusContext!.Status = "Connecting to Designer...";
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
       .Use(async (ctx, next) =>
       {
           ctx.StatusContext!.Status = "Connecting Designer RPC...";
           var sw = Stopwatch.StartNew();
           try
           {
               var attached = JsonRpc.Attach(ctx.DesignerStream!);

               ctx.ModelEditor = new ModelEditor(attached);

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
       .Use(async (ctx, next) =>
       {
           ctx.StatusContext!.Status = "Loading Application Model...";

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
            var result = await ctx.ModelEditor!.Pong();

            AnsiConsole.WriteLine(result);

            await next();
        });

        pipeline.Use(async (ctx, next) =>
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
                    ctx.ExitCode = 1;
                    AnsiConsole.MarkupLine($"[yellow]Added packages. Please restart command to see effects.[/]");
                    return;
                }
            }
            await next();
        })
        .Use(async (ctx, next) =>
        {
            Dictionary<string, (FileState state, SyntaxTree syntaxTree)> modifications = new();
            Dictionary<string, SyntaxTree> originalSyntaxTrees = new();

            List<string> removedViews = new();

            await AnsiConsole.Status().Start("Analyzing views...", async statusContext =>
            {
                statusContext.Spinner(Spinner.Known.Star);
                ctx.StatusContext = statusContext;

                var namespaces = ctx.Settings.Namespaces?.Split(';') ?? Array.Empty<string>();

                var views = await ctx.ModelEditor.GetViewIds(namespaces);

                foreach (var viewId in views)
                {
                    AnsiConsole.MarkupLine($"[grey]Analyzing: [silver][/]{viewId}[/]");

                    var xml = await ctx.ModelEditor.GetViewAsXml(viewId);
                    var viewType = await ctx.ModelEditor.GetViewType(viewId);
                    var modelClass = await ctx.ModelEditor.GetModelClass(viewId);

                    //TODO: move to Design later once we have customizable X2CCode output
                    var codeResult = X2CEngine.ConvertToCode(xml);

                    static Location? FindFile(TPipelineContext ctx, string? modelClass)
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

                        var symbol = ctx.Compilation!.GetTypeByMetadataName(modelClass);

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
                    removedViews.Add(viewId);
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
        })
;

        static SyntaxNode RewriteSyntaxTree(TPipelineContext ctx, ViewType viewType, X2CCodeResult result, SyntaxNode? root, SemanticModel semanticModel)
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
            root = Formatter.Format(root, ctx.Workspace!);
            return root;
        }
    }

}
