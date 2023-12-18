using System;
using System.Linq;

using Microsoft.Extensions.Logging;

using Spectre.Console.Cli;

using Xenial.Cli.Utils;

namespace Xenial.Cli.Commands;

public record CommandInterceptor(LogLevel LogLevel, bool? Wizard = null, bool? NoLogo = null) : ICommandInterceptor
{
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is IBaseSettings baseSettings)
        {
            baseSettings.Verbosity = LogLevel;
            if (!baseSettings.NoLogo)
            {
                if (!NoLogo.HasValue || !NoLogo.Value)
                {
                    BrandHelper.PrintBrandInfo();
                }
            }
            if (Wizard.HasValue)
            {
                baseSettings.RunAsWizard = Wizard.Value;
            }
        }
    }
}
