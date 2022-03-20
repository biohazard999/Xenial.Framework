using System;
using System.Linq;

using Microsoft.Extensions.Logging;

using Spectre.Console.Cli;

using Xenial.Cli.Utils;

namespace Xenial.Cli.Commands;

public record CommandInterceptor(LogLevel LogLevel) : ICommandInterceptor
{
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is IBaseSettings baseSettings)
        {
            baseSettings.Verbosity = LogLevel;
            if (!baseSettings.NoLogo)
            {
                BrandHelper.PrintBrandInfo();
            }
        }
    }
}
