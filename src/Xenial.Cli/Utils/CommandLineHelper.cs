using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.Utils;

public static class CommandLineHelper
{
    public static TOption1? GetGlobalOptions<TOption1>(string[] args, Option<TOption1> option1)
    {
        var (r1, _, _, _) = GetGlobalOptions<TOption1, bool, bool, bool>(args, option1, null, null, null);
        return r1;
    }

    public static (TOption1?, TOption2?) GetGlobalOptions<TOption1, TOption2>(
           string[] args,
           Option<TOption1> option1,
           Option<TOption2>? option2 = null
    )
    {
        var (r1, r2, _, _) = GetGlobalOptions<TOption1, TOption2, bool, bool>(args, option1, option2, null, null);
        return (r1, r2);
    }

    public static (TOption1?, TOption2?, TOption3?) GetGlobalOptions<TOption1, TOption2, TOption3>(
            string[] args,
            Option<TOption1> option1,
            Option<TOption2>? option2 = null,
            Option<TOption3>? option3 = null
    )
    {
        var (r1, r2, r3, _) = GetGlobalOptions<TOption1, TOption2, TOption3, bool>(args, option1, option2, option3, null);
        return (r1, r2, r3);
    }

    public static (TOption1?, TOption2?, TOption3?, TOption4?) GetGlobalOptions<TOption1, TOption2, TOption3, TOption4>(
            string[] args,
            Option<TOption1> option1,
            Option<TOption2>? option2 = null,
            Option<TOption3>? option3 = null,
            Option<TOption4>? option4 = null
    )
    {
        var rootCommand = new System.CommandLine.RootCommand()
        {
            TreatUnmatchedTokensAsErrors = false,
            IsHidden = true,
        };

        static void AddOption(Command command, Option? option)
        {
            if (option is not null)
            {
                command.AddOption(option);
            }
        }

        AddOption(rootCommand, option1);
        AddOption(rootCommand, option2);
        AddOption(rootCommand, option3);
        AddOption(rootCommand, option4);

        var parser = new Parser(rootCommand);

        var result = parser.Parse(args);

        static bool TryGetResult<TResult>(ParseResult? result, Option<TResult>? option, out TResult? r)
        {
            if (result is not null && option is not null && result.FindResultFor(option) is OptionResult optionResult)
            {
                r = optionResult.GetValueForOption(option);
                return true;
            }

            r = default;
            return false;
        }

        TryGetResult(result, option1, out var r1);
        TryGetResult(result, option2, out var r2);
        TryGetResult(result, option3, out var r3);
        TryGetResult(result, option4, out var r4);

        return (r1, r2, r3, r4);
    }


    public class NullConsole : IConsole
    {
        public IStandardStreamWriter Out { get; } = new NullStandardStreamWriter();
        public bool IsOutputRedirected { get; }
        public IStandardStreamWriter Error { get; } = new NullStandardStreamWriter();
        public bool IsErrorRedirected { get; }
        public bool IsInputRedirected { get; }
    }

    public class NullStandardStreamWriter : IStandardStreamWriter
    {
        public void Write(string? value) { }
    }
}
