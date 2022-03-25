
using DevExpress.ExpressApp;

using System;
using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;

namespace Xenial.Design.Contracts;

public interface IModelEditorServer
{
    Task<string?> GetModelClass(string viewId);
    Task<string> GetViewAsXml(string viewId);
    Task<string[]> GetViewIds(IList<string> namespaces);
    Task<ViewType> GetViewType(string viewId);
    Task LaunchDebugger();
    Task<int> LoadModel(string targetFileName, string modelDifferencesStorePath, string deviceSpecificDifferencesStoreName, string? assembliesPath);
    Task<string> Ping();
    Task<string> Pong();
    Task Shutdown();
}

