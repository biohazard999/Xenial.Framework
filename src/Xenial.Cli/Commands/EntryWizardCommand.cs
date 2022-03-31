
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xenial.Cli.DependencyInjection;
using Xenial.Cli.Utils;

namespace Xenial.Cli.Commands;

public interface IBaseSettings
{
    string WorkingDirectory { get; }
    bool NoLogo { get; }
    LogLevel Verbosity { get; set; }
    bool RunAsWizard { get; set; }
}

public class BaseCommandSettings : CommandSettings, IBaseSettings
{
    public BaseCommandSettings(string? workingDirectory)
        => WorkingDirectory = workingDirectory
            ?? Environment.CurrentDirectory;

    [Description("The working directory")]
    [CommandOption("-w|--working-directory")]
    public string WorkingDirectory { get; }

    public override ValidationResult Validate()
        => Directory.Exists(WorkingDirectory)
            ? ValidationResult.Success()
            : ValidationResult.Error($"{nameof(WorkingDirectory)} must exist: {WorkingDirectory}");

    [DefaultValue(LogLevel.Warning)]
    [CommandOption("-v|--verbosity")]
    public LogLevel Verbosity { get; set; }

    [CommandOption("--nologo", IsHidden = true)]
    public bool NoLogo { get; set; }

    [Description("Run as wizard aka. interactive mode")]
    [CommandOption("-z|--wizard", IsHidden = true)]
    public bool RunAsWizard { get; set; }
}

public sealed class EntryWizardCommand : AsyncCommand<BaseCommandSettings>
{
    private readonly Dictionary<string, string> commands = new()
    {
        ["build"] = "Builds a project to inspect with xenial.cli",
        ["model"] = "Converts xafml based views to LayoutBuilder code"
    };

    private readonly IServiceCollection serviceCollection;
    private readonly ICommandlineArgsProvider commandlineArgsProvider;

    public EntryWizardCommand(IServiceCollection serviceCollection, ICommandlineArgsProvider commandlineArgsProvider)
    {
        this.commandlineArgsProvider = commandlineArgsProvider;
        this.serviceCollection = serviceCollection;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, BaseCommandSettings settings)
    {
        _ = settings ?? throw new ArgumentNullException(nameof(settings));

        var choices = commands.Keys;

        var commandName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
              .Title("What [silver]command[/] do you want to execute?")
              .PageSize(10)
              .MoreChoicesText("[grey](Move up and down to reveal commands)[/]")
              .AddChoices(choices)
              .UseConverter(displayString => $"{displayString} - {commands[displayString]}")
            );

        AnsiConsole.WriteLine();

        using var registrar = new DependencyInjectionRegistrar(serviceCollection);

        ICommandApp app = commandName switch
        {
            "build" => new CommandApp<BuildCommand>(registrar),
            "model" => new CommandApp<ModelCommand>(registrar),
            _ => throw new ArgumentOutOfRangeException(nameof(commandName), commandName, "No wizard implemented")
        };

        app.Configure(c =>
        {
            c.SetInterceptor(new CommandInterceptor(settings.Verbosity, Wizard: true, NoLogo: true));
        });

        var args = commandlineArgsProvider.Arguments;
        return await app.RunAsync(args);
    }
}

[TypeConverter(typeof(CommandLineEntryModeTypeConverter))]
public enum CommandLineEntryMode
{
    Wizard = 0,
    Silent = 1,
}

public class CommandLineEntryModeTypeConverter : TypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string sValue)
        {
            CommandLineEntryMode? val = sValue switch
            {
                _ when new[] { "s", "S" }.Contains(sValue) => CommandLineEntryMode.Silent,
                _ when new[] { "w", "W" }.Contains(sValue) => CommandLineEntryMode.Wizard,
                _ => null
            };

            if (val.HasValue)
            {
                return val.Value;
            }
        }

        return base.ConvertFrom(context, culture, value);
    }

}
