using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

using StreamJsonRpc;

using Xenial.Design.Contracts;
using Xenial.Design.Engine;
using Xenial.Framework.DevTools.X2C;

namespace Xenial.Design;

public class ModelEditorServer : IModelEditorServer
{
    public bool Debug { get; set; }

    public JsonRpc? JsonRpc { get; set; }
    public StandaloneModelEditorModelLoader? ModelLoader { get; set; }

    public async Task<string> Ping() => $"Hello from Xenial.Design PID: {Process.GetCurrentProcess().Id}";

    public async Task<int> LoadModel(
        string targetFileName,
        string modelDifferencesStorePath,
        string deviceSpecificDifferencesStoreName,
        string? assembliesPath
    )
    {
        ModelLoader = new StandaloneModelEditorModelLoader();
        try
        {
            await LaunchDebugger();

            ModelLoader.LoadModel(targetFileName, modelDifferencesStorePath, deviceSpecificDifferencesStoreName, assembliesPath);

            return ModelLoader.ModelApplication!.Views.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return -1;
        }
    }

    public async Task LaunchDebugger()
    {
        if (Debug)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
        }
    }

    public async Task<IList<string>> GetViewIds(IList<string> namespaces)
    {
        static IEnumerable<string> IterateViewIds(IModelApplication application, IList<string> namespaces)
        {
            foreach (var view in application.Views)
            {
                static bool AcceptsModelView(IModelObjectView modelObjectView, IList<string> namespaces)
                {
                    if (namespaces.Count == 0)
                    {
                        return true;
                    }

                    var ns = modelObjectView.ModelClass.TypeInfo.Type.Namespace ?? "";

                    return namespaces.Any(ns => ns.StartsWith(ns, StringComparison.OrdinalIgnoreCase));
                }

                //TODO: Dashboard views
                if (view is IModelObjectView modelObjectView
                    && AcceptsModelView(modelObjectView, namespaces)
                    )
                {
                    yield return view.Id;
                }
            }
        }

        return IterateViewIds(ModelLoader!.ModelApplication!, namespaces)
            .ToArray();
    }

    public async Task<ViewType> GetViewType(string viewId)
    {
        static ViewType FindViewType(IModelApplication modelApplication, string viewId)
        {
            var view = modelApplication.Views[viewId];
            return view switch
            {
                IModelDetailView => ViewType.DetailView,
                IModelListView => ViewType.ListView,
                IModelDashboardView => ViewType.DashboardView,
                _ => ViewType.Any,
            };
        }

        return FindViewType(ModelLoader!.ModelApplication!, viewId);
    }

    public async Task<string> GetViewAsXml(string viewId)
    {
        static string ConvertViewToXml(IModelApplication modelApplication, string viewId)
        {
            var view = modelApplication.Views[viewId];

            var xml = X2CEngine.ConvertToXml(view, false);

            return xml;
        }

        return ConvertViewToXml(ModelLoader!.ModelApplication!, viewId);
    }

    public async Task<string?> GetModelClass(string viewId)
    {
        static string? FindModelClass(IModelApplication modelApplication, string viewId)
        {
            var view = modelApplication.Views[viewId];
            if (view is IModelObjectView modelObjectView)
            {
                return modelObjectView.ModelClass.Name;
            }
            return null;
        }

        return FindModelClass(ModelLoader!.ModelApplication!, viewId);
    }

    public async Task Shutdown()
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

    public async Task<string?> GetModelFileName() => ModelLoader?.FileModelStore?.Name;
}
