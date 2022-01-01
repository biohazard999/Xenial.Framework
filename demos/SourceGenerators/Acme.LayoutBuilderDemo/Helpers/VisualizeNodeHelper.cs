using System;
using System.Linq;

using Acme.Module.Helpers.Xml;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;


namespace Acme.Module.Helpers;

#nullable enable

internal static class VisualizeNodeHelper
{
    public static void PrintModelNode(this IModelNode modelNode)
    {
        var xml = UserDifferencesHelper.GetUserDifferences(modelNode)[""];
        var prettyXml = new XmlFormatter().Format(xml);
        Console.WriteLine(prettyXml);
    }
}
