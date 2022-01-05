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
    {
        var writer = new ModelXmlWriter();
        return writer.WriteToString(modelNode, 0);
    }
}
