using System;
using System.Linq;

using Acme.Module.Helpers.Xml;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;


namespace Acme.Module.Helpers;

#nullable enable

internal static class VisualizeNodeHelper
{
    public static string PrintModelNode(this IModelNode modelNode)
        => UserDifferencesHelper.GetUserDifferences(modelNode)[""];

    public static string PrettyPrint(string xml)
    {
        var prettyXml = new XmlFormatter().Format(xml);
        return prettyXml;
    }
}
