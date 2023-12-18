using System;
using System.Diagnostics;
using System.Linq;

namespace Xenial.Cli.Engine;

public record PipelineContext : IDisposable
{
    public Stopwatch Stopwatch { get; set; } = Stopwatch.StartNew();
    public int? ExitCode { get; set; }

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
