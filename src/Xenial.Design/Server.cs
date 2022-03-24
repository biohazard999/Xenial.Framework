using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

using StreamJsonRpc;

using Xenial.Cli.Engine;
using Xenial.Framework.DevTools.X2C;

namespace Xenial.Design;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
public class ModelEditorServer
{
    public bool Debug { get; set; }

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
            LaunchDebugger();

            ModelLoader.LoadModel(targetFileName, modelDifferencesStorePath, deviceSpecificDifferencesStoreName, assembliesPath);

            return ModelLoader.ModelApplication!.Views.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return -1;
        }
    }

    [Conditional("DEBUG")]
    private void LaunchDebugger()
    {
        if (Debug)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
        }
    }

    public string[] GetViewIds(IList<string> namespaces!!)
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

                    var @namespace = modelObjectView.ModelClass.TypeInfo.Type.Namespace ?? "";

                    return namespaces.Any(ns => @namespace.StartsWith(ns, StringComparison.OrdinalIgnoreCase));
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

    public ViewType GetViewType(string viewId)
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
    //modelObjectView.ModelClass.Name

    public string GetViewAsXml(string viewId)
    {
        static string ConvertViewToXml(IModelApplication modelApplication, string viewId)
        {
            var view = modelApplication.Views[viewId];

            var xml = X2CEngine.ConvertToXml(view);

            return xml;
        }

        return ConvertViewToXml(ModelLoader!.ModelApplication!, viewId);
    }

    public string? GetModelClass(string viewId)
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
