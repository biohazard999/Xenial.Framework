using Spectre.Console;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.Utils;

public static class BrandHelper
{
    public static void PrintBrandInfo()
        => AnsiConsole.Write(
            new FigletText("xenial.io")
            .Centered()
            .Color(new Color(red: 56, green: 188, blue: 216)));
}
