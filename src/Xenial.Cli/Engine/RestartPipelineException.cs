using System.Runtime.Serialization;

namespace Xenial.Cli.Engine;

public sealed class RestartPipelineException : Exception
{
    public RestartPipelineException(string str) : base(str)
    {
    }
}
