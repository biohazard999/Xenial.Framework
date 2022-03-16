using Spectre.Console;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

using Color = Spectre.Console.Color;
using FontStyle = TextMateSharp.Themes.FontStyle;

namespace Xenial.Cli.Utils;

public static class ConsoleHelper
{
    public static void Success(this Stopwatch sw!!, string caption)
    {
        sw.Stop();
        AnsiConsole.MarkupLine($"[green][[SUCCESS]]        [/][grey]: [/][silver]{caption}[/] [grey][[{sw.Elapsed}]][/]");
        sw.Restart();
    }

    public static void Warn(this Stopwatch sw!!, string caption)
    {
        sw.Stop();
        AnsiConsole.MarkupLine($"[red][[WARNING]]        [/][grey]: [/][red]{caption}[/] [silver][[{sw.Elapsed}]][/]");
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


    public static void PrintSource(string code, string lang = "cs")
    {
        using var sr = new StringReader(code);
        PrintSource(sr, lang);
    }

    private static Lazy<(IGrammar grammar, Theme theme)> TextMateContextCSharp { get; } = new Lazy<(IGrammar grammer, Theme theme)>(() =>
    {
        var options = new RegistryOptions(ThemeName.DarkPlus);

        var registry = new Registry(options);

        var theme = registry.GetTheme();

        var grammar = registry.LoadGrammar(options.GetScopeByExtension(".cs"));
        return (grammar, theme);
    });

    private static Lazy<(IGrammar grammar, Theme theme)> TextMateContextXml { get; } = new Lazy<(IGrammar grammer, Theme theme)>(() =>
    {
        var options = new RegistryOptions(ThemeName.DarkPlus);

        var registry = new Registry(options);

        var theme = registry.GetTheme();

        //This throws NRE in Textmate 1.0.31. So use the scope name direct
        //var scope = options.GetScopeByExtension(".xml");
        //var grammar = registry.LoadGrammar(scope);
        var grammar = registry.LoadGrammar("text.xml");
        return (grammar, theme);
    });

    private static void PrintSource(StringReader sr, string lang = "cs")
    {
        var (grammar, theme) = lang switch
        {
            "cs" => TextMateContextCSharp.Value,
            "xml" => TextMateContextXml.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(lang), lang, $"Could not find grammer for language '{lang}'.")
        };

        StackElement? ruleStack = null;

        var line = sr.ReadLine();

        while (line is not null)
        {
            var result = grammar.TokenizeLine(line, ruleStack);

            ruleStack = result.RuleStack;

            foreach (var token in result.Tokens)
            {
                var startIndex = (token.StartIndex > line.Length) ?
                    line.Length : token.StartIndex;
                var endIndex = (token.EndIndex > line.Length) ?
                    line.Length : token.EndIndex;

                var foreground = -1;
                var background = -1;
                var fontStyle = -1;

                foreach (var themeRule in theme.Match(token.Scopes))
                {
                    if (foreground == -1 && themeRule.foreground > 0)
                    {
                        foreground = themeRule.foreground;
                    }

                    if (background == -1 && themeRule.background > 0)
                    {
                        background = themeRule.background;
                    }

                    if (fontStyle == -1 && themeRule.fontStyle > 0)
                    {
                        fontStyle = themeRule.fontStyle;
                    }
                }

                WriteToken(line.SubstringAtIndexes(startIndex, endIndex), foreground, background, fontStyle, theme);
            }

            AnsiConsole.WriteLine();
            line = sr.ReadLine();
        }
    }

    private static void WriteToken(string text, int foreground, int background, int fontStyle, Theme theme)
    {
        if (foreground == -1)
        {
#pragma warning disable Spectre1000 // Use AnsiConsole instead of System.Console
            Console.Write(text);
#pragma warning restore Spectre1000
            return;
        }

        var decoration = GetDecoration(fontStyle);

        var backgroundColor = GetColor(background, theme);
        var foregroundColor = GetColor(foreground, theme);

        var style = new Style(foregroundColor, backgroundColor, decoration);
        var markup = new Markup(text
            .Replace("[", "[[", StringComparison.OrdinalIgnoreCase)
            .Replace("]", "]]", StringComparison.OrdinalIgnoreCase),
            style
        );

        AnsiConsole.Write(markup);
    }

    private static Color GetColor(int colorId, Theme theme)
    {
        if (colorId == -1)
        {
            return Color.Default;
        }

        return HexToColor(theme.GetColor(colorId));
    }

    private static Decoration GetDecoration(int fontStyle)
    {
        var result = Decoration.None;

        if (fontStyle == FontStyle.NotSet)
        {
            return result;
        }

        if ((fontStyle & FontStyle.Italic) != 0)
        {
            result |= Decoration.Italic;
        }

        if ((fontStyle & FontStyle.Underline) != 0)
        {
            result |= Decoration.Underline;
        }

        if ((fontStyle & FontStyle.Bold) != 0)
        {
            result |= Decoration.Bold;
        }

        return result;
    }

    private static Color HexToColor(string hexString)
    {
        //replace # occurences
        if (hexString.IndexOf('#', StringComparison.OrdinalIgnoreCase) != -1)
        {
            hexString = hexString.Replace("#", "", StringComparison.OrdinalIgnoreCase);
        }

#pragma warning disable CA1305 // Specify IFormatProvider
        var r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
        var g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
        var b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);
#pragma warning restore CA1305 // Specify IFormatProvider

        return new Color(r, g, b);
    }
}

internal static class StringExtensions
{
    internal static string SubstringAtIndexes(this string str, int startIndex, int endIndex)
        => str.Substring(startIndex, endIndex - startIndex);
}
