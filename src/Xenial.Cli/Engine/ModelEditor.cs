
using StreamJsonRpc;

using System;

using System.Linq;

using Xenial.Design;

namespace Xenial.Cli.Engine;

public class ModelEditor : IDisposable
{
    private readonly JsonRpc server;
    private bool disposedValue;

    public bool IsDisposed => disposedValue;

    public ModelEditor(JsonRpc server)
        => this.server = server;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _ = Shutdown();

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

    public Task<string> Ping() => server.InvokeAsync<string>(nameof(ModelEditorServer.Ping));
    public Task<string> Pong() => server.InvokeAsync<string>(nameof(ModelEditorServer.Pong));

    public Task Shutdown() => server.InvokeAsync(nameof(ModelEditorServer.Shutdown));
    public Task<int> LoadModel(string assemblyPath, string folder, string deviceSpecificDifferencesStoreName, string targetDir)
        => server.InvokeAsync<int>(nameof(ModelEditorServer.LoadModel), assemblyPath, folder, deviceSpecificDifferencesStoreName, targetDir);
}
