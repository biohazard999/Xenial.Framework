using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Services;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Xenial.FeatureCenter.Blazor.Server
{
    internal static class Program
    {
        private static bool ContainsArgument(string[] args, string argument)
            => args.Any(arg => arg.TrimStart('/').TrimStart('-').ToUpperInvariant() == argument.ToUpperInvariant());

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
              .MinimumLevel.Override("System", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
              .Enrich.FromLogContext()
              .WriteTo.File(
                  @"C:\logs\framework.featurecenter.xenial.io\framework.featurecenter.xenial.io.log",
                  fileSizeLimitBytes: 1_000_000,
                  rollOnFileSizeLimit: true,
                  shared: true,
                  flushToDiskInterval: TimeSpan.FromSeconds(1))
              .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
              .CreateLogger();

            try
            {
                Log.Information("Starting host...");
                if (ContainsArgument(args, "help") || ContainsArgument(args, "h"))
                {
                    Console.WriteLine("Updates the database when its version does not match the application's version.");
                    Console.WriteLine();
                    Console.WriteLine($"    {Assembly.GetExecutingAssembly().GetName().Name}.exe --updateDatabase [--forceUpdate --silent]");
                    Console.WriteLine();
                    Console.WriteLine("--forceUpdate - Marks that the database must be updated whether its version matches the application's version or not.");
                    Console.WriteLine("--silent - Marks that database update proceeds automatically and does not require any interaction with the user.");
                    Console.WriteLine();
                    Console.WriteLine($"Exit codes: 0 - {DBUpdater.StatusUpdateCompleted}");
                    Console.WriteLine($"            1 - {DBUpdater.StatusUpdateError}");
                    Console.WriteLine($"            2 - {DBUpdater.StatusUpdateNotNeeded}");
                }
                else
                {
                    FrameworkSettings.DefaultSettingsCompatibilityMode = FrameworkSettingsCompatibilityMode.Latest;
                    var host = CreateHostBuilder(args).Build();
                    if (ContainsArgument(args, "updateDatabase"))
                    {
                        using var serviceScope = host.Services.CreateScope();
                        return serviceScope.ServiceProvider.GetRequiredService<IDBUpdater>().Update(ContainsArgument(args, "forceUpdate"), ContainsArgument(args, "silent"));
                    }
                    else
                    {
                        Log.Information("Host run...");
                        host.Run();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
