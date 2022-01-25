using System;
using System.Linq;
using System.Xml.Linq;

using Acme.Module.Helpers.Xml;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;


namespace Acme.Module.Helpers;

#nullable enable

internal static class VisualizeNodeHelper
{
    public static string PrintModelNode(this IModelNode modelNode)
        => UserDifferencesHelper.GetUserDifferences(modelNode)[""];

    public static string PrettyPrint(string xml, bool prettyPrint = true)
    {
        if (prettyPrint)
        {
            var prettyXml = new XmlFormatter().Format(xml);
            return prettyXml;
        }
        else
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                // Handle and throw if fatal exception here; don't just ignore them
                return xml;
            }
        }
    }
}
