
using DevExpress.ExpressApp;

using StreamJsonRpc;

using System;

using System.Linq;

using Xenial.Design.Contracts;

namespace Xenial.Cli.Engine;

public class ModelEditorRpcClient : IDisposable, IModelEditorServer
{
    private readonly JsonRpc server;
    private bool disposedValue;

    public bool IsDisposed => disposedValue;

    public ModelEditorRpcClient(JsonRpc server)
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

    public Task<string> Ping()
        => server.InvokeAsync<string>(nameof(Ping));

    public Task<string> Pong()
        => server.InvokeAsync<string>(nameof(Pong));

    public Task Shutdown()
        => server.InvokeAsync(nameof(Shutdown));

    public Task<int> LoadModel(string assemblyPath, string folder, string deviceSpecificDifferencesStoreName, string targetDir)
        => server.InvokeAsync<int>(nameof(LoadModel), assemblyPath, folder, deviceSpecificDifferencesStoreName, targetDir);

    public Task<IList<string>> GetViewIds(IList<string> namespaces)
        => server.InvokeAsync<IList<string>>(nameof(GetViewIds), namespaces);

    public Task<string> GetViewAsXml(string viewId)
        => server.InvokeAsync<string>(nameof(GetViewAsXml), viewId);

    public Task<string?> GetModelClass(string viewId)
        => server.InvokeAsync<string?>(nameof(GetModelClass), viewId);

    public Task<ViewType> GetViewType(string viewId)
        => server.InvokeAsync<ViewType>(nameof(GetViewType), viewId);

    public Task LaunchDebugger()
        => server.InvokeAsync(nameof(LaunchDebugger));
}
