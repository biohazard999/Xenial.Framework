using Spectre.Console;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.Utils;

public static class ConsoleHelper
{
    public static void Success(this Stopwatch sw!!, string caption)
    {
        sw.Stop();
        AnsiConsole.MarkupLine($"[green][[SUCCESS]]        [/][grey]: [/][silver]{caption}[/] [grey][[{sw.Elapsed}]][/]");
        sw.Restart();
    }

    public static void Fail(this Stopwatch sw!!, string caption)
    {
        sw.Stop();
        AnsiConsole.MarkupLine($"[red][[FAILURE]]        [/][grey]: [/][red]{caption}[/] [silver][[{sw.Elapsed}]][/]");
        sw.Restart();
    }

    public static void HorizontalRule(string title)
    {
        AnsiConsole.Write(new Rule($"[white bold]{title}[/]").RuleStyle("grey").LeftAligned());
        AnsiConsole.WriteLine();
    }

    public static void HorizontalDashed(string title)
    {
        AnsiConsole.Write(new Rule($"[white]{title}[/]").RuleStyle("grey").LeftAligned().AsciiBorder());
        AnsiConsole.WriteLine();
    }

    public static void WritePath(string path)
    {
        path = EllipsisPath(path, 80);

        AnsiConsole.Write(
            new TextPath(path)
                .RootColor(Color.White)
                .SeparatorColor(Color.Grey)
                .StemColor(Color.White)
                .LeafColor(Color.Green)
        );
    }

    public static string EllipsisPath(this string rawString!!, int maxLength = 80, char delimiter = '\\')
    {
        maxLength -= 3; //account for delimiter spacing

        if (rawString.Length <= maxLength)
        {
            return rawString;
        }

        List<string> parts;

        var loops = 0;
        while (loops++ < 100)
        {
            parts = rawString.Split(delimiter).ToList();
            parts.RemoveRange(parts.Count - 1 - loops, loops);
            if (parts.Count == 1)
            {
                return parts.Last();
            }

            parts.Insert(parts.Count - 1, "...");
            var final = string.Join(delimiter.ToString(), parts);
            if (final.Length < maxLength)
            {
                return final;
            }
        }

        return rawString.Split(delimiter).ToList().Last();
    }
}
