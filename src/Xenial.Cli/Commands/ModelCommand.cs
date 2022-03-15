using Buildalyzer;
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
using Microsoft.CodeAnalysis;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;
using System.Globalization;

namespace Xenial.Cli.Commands;

public class ModelCommandSettings : BuildCommandSettings
{
    public ModelCommandSettings(string workingDirectory) : base(workingDirectory) { }

    [Description("Specifies the target framework to load the model from. Is required when the project is multi-targeted.")]
    [CommandOption("-f|--tfm")]
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
    public Compilation? Compilation { get; set; }
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
               ctx.Compilation = await project.GetCompilationAsync();

               var type = ctx.Compilation!.GetTypeByMetadataName("Xenial.FeatureCenter.Module.Model.GeneratorUpdaters.FeatureCenterNavigationItemNodesUpdater");
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

    protected override void ConfigurePipeline(TPipeline pipeline!!)
    {
        base.ConfigurePipeline(pipeline);

        pipeline.Use(async (ctx, next) =>
        {
            foreach (var view in ctx.Application!.Views)
            {
                var code = Xenial.Framework.DevTools.X2C.X2CEngine.ConvertToCode(view);

                PrintSource(code);
            }

            await next();
        });
    }



    private static void PrintSource(string code)
    {
        using var sr = new StringReader(code);
        PrintSource(sr);
    }

    private static Lazy<(IGrammar grammar, Theme theme)> TextMateContextCSharp { get; } = new Lazy<(IGrammar grammer, Theme theme)>(() =>
    {
        var options = new RegistryOptions(ThemeName.DarkPlus);

        var registry = new Registry(options);

        var theme = registry.GetTheme();

        var grammar = registry.LoadGrammar(options.GetScopeByExtension(".cs"));
        return (grammar, theme);
    });

    private static void PrintSource(StringReader sr)
    {
        var (grammar, theme) = TextMateContextCSharp.Value;

        StackElement? ruleStack = null;

        var line = sr.ReadLine();

        while (line is not null)
        {
            var result = grammar.TokenizeLine(line, ruleStack);

            ruleStack = result.RuleStack;

            foreach (var token in result.Tokens)
            {
                var startIndex = (token.StartIndex > line.Length) ?
                    line.Length : token.StartIndex;
                var endIndex = (token.EndIndex > line.Length) ?
                    line.Length : token.EndIndex;

                var foreground = -1;
                var background = -1;
                var fontStyle = -1;

                foreach (var themeRule in theme.Match(token.Scopes))
                {
                    if (foreground == -1 && themeRule.foreground > 0)
                    {
                        foreground = themeRule.foreground;
                    }

                    if (background == -1 && themeRule.background > 0)
                    {
                        background = themeRule.background;
                    }

                    if (fontStyle == -1 && themeRule.fontStyle > 0)
                    {
                        fontStyle = themeRule.fontStyle;
                    }
                }

                WriteToken(line.SubstringAtIndexes(startIndex, endIndex), foreground, background, fontStyle, theme);
            }

            AnsiConsole.WriteLine();
            line = sr.ReadLine();
        }
    }

    private static void WriteToken(string text, int foreground, int background, int fontStyle, Theme theme)
    {
        if (foreground == -1)
        {
            Console.Write(text);
            return;
        }

        var decoration = GetDecoration(fontStyle);

        var backgroundColor = GetColor(background, theme);
        var foregroundColor = GetColor(foreground, theme);

        var style = new Style(foregroundColor, backgroundColor, decoration);
        var markup = new Markup(text.Replace("[", "[[").Replace("]", "]]"), style);

        AnsiConsole.Write(markup);
    }

    private static Color GetColor(int colorId, Theme theme)
    {
        if (colorId == -1)
        {
            return Color.Default;
        }

        return HexToColor(theme.GetColor(colorId));
    }

    private static Decoration GetDecoration(int fontStyle)
    {
        var result = Decoration.None;

        if (fontStyle == FontStyle.NotSet)
        {
            return result;
        }

        if ((fontStyle & FontStyle.Italic) != 0)
        {
            result |= Decoration.Italic;
        }

        if ((fontStyle & FontStyle.Underline) != 0)
        {
            result |= Decoration.Underline;
        }

        if ((fontStyle & FontStyle.Bold) != 0)
        {
            result |= Decoration.Bold;
        }

        return result;
    }

    private static Color HexToColor(string hexString)
    {
        //replace # occurences
        if (hexString.IndexOf('#') != -1)
        {
            hexString = hexString.Replace("#", "");
        }

        byte r, g, b = 0;

        r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
        g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
        b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

        return new Color(r, g, b);
    }
}

internal static class StringExtensions
{
    internal static string SubstringAtIndexes(this string str, int startIndex, int endIndex)
        => str.Substring(startIndex, endIndex - startIndex);
}
