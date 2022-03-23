using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamJsonRpc;

using Xenial.Cli.Engine;

namespace Xenial.Design;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
public class ModelEditorServer
{
    public JsonRpc? JsonRpc { get; set; }
    public StandaloneModelEditorModelLoader? ModelLoader { get; set; }

    public string Ping() => $"Hello from Server {Guid.NewGuid()}";
    public string Pong() => $"Pong from Server {Guid.NewGuid()}";

    public int LoadModel(
        string targetFileName,
        string modelDifferencesStorePath,
        string deviceSpecificDifferencesStoreName,
        string? assembliesPath
    )
    {
        ModelLoader = new StandaloneModelEditorModelLoader();
        try
        {
#if DEBUG
            Debugger.Launch();
#endif
            ModelLoader.LoadModel(targetFileName, modelDifferencesStorePath, deviceSpecificDifferencesStoreName, assembliesPath);

            return ModelLoader.ModelApplication!.Views.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return -1;
        }
    }

    public void Shutdown()
    {
        try
        {
            Console.WriteLine("Shutting Down...");
            JsonRpc?.Dispose();
        }
        finally
        {
            Environment.Exit(0);
        }
    }
}
