using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.Utils;

public interface ICommandlineArgsProvider
{
    string[] Arguments { get; }
}

public class CommandlineArgsProvider : ICommandlineArgsProvider
{
    public string[] Arguments { get; }

    public CommandlineArgsProvider(string[] arguments)
        => Arguments = arguments;
}
