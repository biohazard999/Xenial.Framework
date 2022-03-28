using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console.Cli;

using System.CommandLine;
using System.Diagnostics;
using System.Text;

using Xenial.Cli.Commands;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var logVerbosityOption = new Option<LogLevel?>(new[] { "-v", "--verbosity" });
var logFileOption = new Option<bool?>(new[] { "--log-file" });
var workingDirectoryOption = new Option<string?>(new[] { "-w", "--working-directory" });
var debugOption = new Option<bool?>(new[] { "--debug" });

var logLvl = Xenial.Cli.Utils.CommandLineHelper.GetGlobalOptions(args, logVerbosityOption) ?? LogLevel.Error;
var logFile = Xenial.Cli.Utils.CommandLineHelper.GetGlobalOptions(args, logFileOption) ?? false;
var workingDirectory = Xenial.Cli.Utils.CommandLineHelper.GetGlobalOptions(args, workingDirectoryOption);
var debug = Xenial.Cli.Utils.CommandLineHelper.GetGlobalOptions(args, debugOption);

if (debug.HasValue && debug.Value)
{
    if (!Debugger.IsAttached)
    {
        Debugger.Launch();
    }
}

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    if (logFile)
    {
        builder.AddFile(c =>
        {
            c.RootPath = workingDirectory ?? Environment.CurrentDirectory;
            c.MaxFileSize = 10_000_000;
            c.FileAccessMode = Karambolo.Extensions.Logging.File.LogFileAccessMode.KeepOpenAndAutoFlush;
            c.Files = new[]
            {
                new Karambolo.Extensions.Logging.File.LogFileOptions { Path = "Xenial.Cli.log" }
            };
        });
    }
}).Configure<LoggerFilterOptions>(options => options.MinLevel = logLvl);

using var registrar = new Xenial.Cli.DependencyInjection.DependencyInjectionRegistrar(services);

var app = new CommandApp
#if DEBUG
    <Xenial.Cli.Commands.EntryWizardCommand>
#endif
(registrar);

app.Configure(c =>
{
    //c.SetInterceptor(new CommandInterceptor(logLvl));
    c.SetApplicationName("xenial");
    c.ValidateExamples();

    c.AddCommand<Xenial.Cli.Commands.BuildCommand>("build");
    c.AddCommand<Xenial.Cli.Commands.ModelCommand>("model");
});

return app.Run(args);
