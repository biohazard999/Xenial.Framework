using System;
using System.Linq;

using ModelToCodeConverter.Engine;

namespace Xenial.Cli.Commands;

public abstract record BuildPipeline<TContext, TSettings> : Pipeline<TContext>
    where TContext : BuildContext<TSettings>
    where TSettings : BuildCommandSettings
{
}

public record BuildContext : BuildContext<BuildCommandSettings>
{

}

public record BuildPipeline : BuildPipeline<BuildContext, BuildCommandSettings>
{
    public override BuildContext CreateContext() => new();
}
