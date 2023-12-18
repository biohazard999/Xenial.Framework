using Spectre.Console;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = Spectre.Console.Color;

namespace Xenial.Cli.Utils;

public static class BrandHelper
{
    public static void PrintBrandInfo()
    {
        var color = new Color(red: 56, green: 188, blue: 216);
        AnsiConsole.Write(
           new FigletText("xenial.io")
           .Centered()
           .Color(color));

        var rule = new Rule($"[silver]Xenial: {XenialVersion.Version.EscapeMarkup()}   DevExpress: {XenialVersion.DxVersion.EscapeMarkup()}[/]");
        rule.RuleStyle(new Style(color));
        AnsiConsole.Write(rule);
    }
}
