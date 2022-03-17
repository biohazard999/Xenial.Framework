
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

namespace Xenial.Cli.Commands;

public interface IBaseSettings
{
    string WorkingDirectory { get; }
    bool NoLogo { get; }
    public LogLevel Verbosity { get; set; }
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
            ? ValidationResult.Error($"{nameof(WorkingDirectory)} must exist: {WorkingDirectory}")
            : ValidationResult.Success();

    [DefaultValue(LogLevel.Warning)]
    [CommandOption("-v|--verbosity")]
    public LogLevel Verbosity { get; set; }

    [CommandOption("--nologo"
    //    , IsHidden = true //TODO: is not available in 0.4.3, wait for later version
    )]
    public bool NoLogo { get; set; }

    [Description("Run as wizard aka. interactive mode")]
    [CommandOption("-z|--wizard"
    //    , IsHidden = true //TODO: is not available in 0.4.3, wait for later version
    )]
    public bool RunAsWizard { get; set; }
}

public sealed class EntryWizardCommand : Command<BaseCommandSettings>
{
    private List<string> commands = new()
    {
        "build",
        "model"
    };

    private readonly IServiceProvider serviceProvider;

    public EntryWizardCommand(IServiceProvider serviceProvider) 
        => this.serviceProvider = serviceProvider;

    public override int Execute([NotNull] CommandContext context, [NotNull] BaseCommandSettings settings)
    {
        var fruit = AnsiConsole.Prompt(
             new SelectionPrompt<string>()
                 .Title("What [silver]command[/] do you want to execute?")
                 .PageSize(10)
                 //.MoreChoicesText("[grey](Move up and down to reveal commands)[/]")
                 .AddChoices(commands));

        // Echo the fruit back to the terminal
        AnsiConsole.WriteLine($"I agree. {fruit} is tasty!");
        AnsiConsole.WriteLine($"I agree. {settings.WorkingDirectory} is tasty!");

        return 0;
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
