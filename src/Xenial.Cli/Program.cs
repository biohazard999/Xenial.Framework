using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


using Spectre.Console.Cli;

using System.CommandLine;

using Xenial.Cli.Commands;
using Xenial.Cli.Utils;

var option = new Option<LogLevel?>(new[] { "-v", "--verbosity" });

var logLvl = Xenial.Cli.Utils.CommandLineHelper.GetGlobalOptions(args, option) ?? LogLevel.Error;

var services = new ServiceCollection();

services.AddLogging(configure =>
{
    configure.AddConsole();
}).Configure<LoggerFilterOptions>(options => options.MinLevel = logLvl);

using var registrar = new Xenial.Cli.DependencyInjection.DependencyInjectionRegistrar(services);

var app = new CommandApp<Xenial.Cli.Commands.EntryWizardCommand>(registrar);

app.Configure(c =>
{
    c.SetInterceptor(new CommandInterceptor(logLvl));
    c.SetApplicationName("xenial");
    c.ValidateExamples();

    c.AddCommand<Xenial.Cli.Commands.BuildCommand>("build");
    c.AddCommand<Xenial.Cli.Commands.ModelCommand>("model");
});

return app.Run(args);

public class CommandInterceptor : ICommandInterceptor
{
    readonly LogLevel logLevel;

    public CommandInterceptor(LogLevel logLevel)
        => this.logLevel = logLevel;

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is IBaseSettings baseSettings)
        {
            baseSettings.Verbosity = logLevel;
            if (!baseSettings.NoLogo)
            {
                BrandHelper.PrintBrandInfo();
            }
        }
    }
}


/// <summary>
/// https://stackoverflow.com/questions/42862739/how-to-copy-files-to-output-directory-from-a-referenced-nuget-package-in-net-co
/// </summary>
/// 

//var sw = new Stopwatch();
//sw.Start();

//var workingDirectory = @"C:\f\git\Xenial.Framework\demos\FeatureCenter\Xenial.FeatureCenter.Module";

//var csProjects = Directory.GetFiles(workingDirectory, "*.csproj");

//if (csProjects.Length > 1)
//{
//    Console.WriteLine($"More than once csproj in the specific location: {workingDirectory}");
//    return 0;
//}

//var csProj = csProjects.First();

//AnalyzerManager manager = new AnalyzerManager();
//var analyzer = manager.GetProject(csProj);

//if (analyzer.ProjectFile.IsMultiTargeted) //TODO: Multitargetig
//{
//    var tfm = analyzer.ProjectFile.TargetFrameworks.First();

//}

//var result = analyzer.Build(new EnvironmentOptions
//{
//    Restore = true,
//    DesignTime = true
//});


//if (result.OverallSuccess)
//{
//    var tfm = "net462";
//    var net6Result = result.First(m => m.TargetFramework == tfm); //TODO: find correct result

//    result = analyzer.Build(net6Result.TargetFramework, new EnvironmentOptions
//    {
//        Restore = true,
//        DesignTime = false,
//        GlobalProperties =
//        {
//            ["CopyLocalLockFileAssemblies"] = "true"
//        },
//    });

//    net6Result = result.First(m => m.TargetFramework == tfm); //TODO: find correct result

//    var assemblyPath = net6Result.Properties["TargetPath"];
//    var targetDir = Path.GetDirectoryName(assemblyPath);
//    var loader = new StandaloneModelEditorModelLoader();

//    loader.LoadModel(
//        assemblyPath,
//        workingDirectory,
//        "",
//        targetDir
//    );

//    sw.Stop();

//    Console.WriteLine($"Loaded Model: {loader.ModelApplication.Views.Count}");
//    Console.WriteLine($"Time: {sw.Elapsed}");

//    var view = loader.ModelApplication.Views[0] as IModelView;

//    var id = $"{view.Id}_{Guid.NewGuid()}";
//    var modelViews = loader.ModelApplication.Views;
//    var copy = ((ModelNode)modelViews).AddClonedNode((ModelNode)view, id);

//    var xml = UserDifferencesHelper.GetUserDifferences(copy)[""];

//    xml = xml.Replace(id, view.Id); //Patch ViewId

//    var doc = new System.Xml.XmlDocument();
//    doc.LoadXml(xml);
//    var root = doc.FirstChild;

//    CleanNodes(root);

//    static void CleanNodes(System.Xml.XmlNode node)
//    {
//        if (node.Attributes["IsNewNode"] != null)
//        {
//            node.Attributes.Remove(node.Attributes["IsNewNode"]);
//        }

//        if (node.Attributes["ShowCaption"] != null)
//        {
//            var value = node.Attributes["ShowCaption"].Value;
//            if (string.IsNullOrEmpty(value)) //Empty boolean should be ignored
//            {
//                node.Attributes.Remove(node.Attributes["ShowCaption"]);
//            }
//        }

//        foreach (System.Xml.XmlNode child in node.ChildNodes)
//        {
//            CleanNodes(child);
//        }
//    }

//    xml = doc.OuterXml;
//    xml = VisualizeNodeHelper.PrettyPrint(xml, true);

//    ((IModelView)copy).Remove();

//    Console.WriteLine(xml);

//    Console.ReadLine();
//}
//return 0;