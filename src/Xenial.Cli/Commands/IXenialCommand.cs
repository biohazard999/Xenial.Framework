using Spectre.Console.Cli;

using System;
using System.Linq;

namespace Xenial.Cli.Commands;

public interface IXenialCommand : ICommand
{
    string CommandName { get; }
}
