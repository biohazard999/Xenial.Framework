using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamJsonRpc;

namespace Xenial.Cli.Engine;

public class ModelEditor : IDisposable
{
    private readonly JsonRpc server;
    private bool disposedValue;

    public ModelEditor(JsonRpc server)
        => this.server = server;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                server.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public Task<string> Ping() => server.InvokeAsync<string>("Ping");
}
