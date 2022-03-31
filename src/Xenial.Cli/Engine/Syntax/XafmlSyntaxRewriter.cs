using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using DevExpress.ExpressApp;

using Xenial.Framework.DevTools.X2C;

namespace Xenial.Cli.Engine.Syntax;

public class XafmlSyntaxRewriter
{
    public string? FileName { get; }
    public IList<string> RemovedViews { get; }

    public XafmlSyntaxRewriter(string? fileName, IList<string> removedViews!!)
    {
        FileName = fileName;
        RemovedViews = removedViews;
    }

    private XmlDocument? doc;

    public async Task<(bool hasModifications, string? modelFilePath)> RewriteAsync()
    {
        if (FileName is null || !File.Exists(FileName))
        {
            return (false, null);
        }

        var content = await File.ReadAllTextAsync(FileName);
        doc = new XmlDocument() { XmlResolver = null! };

        var sreader = new System.IO.StringReader(content);
        using var reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null });
        doc.Load(reader);

        var views = doc["Application"]?["Views"];

        var modified = false;

        if (views is not null)
        {
            foreach (var removedViewId in RemovedViews)
            {
                var view = views.OfType<XmlNode>().FirstOrDefault(m => m.Attributes is not null && m.Attributes["Id"]?.Value == removedViewId);
                if (view is not null)
                {
                    views.RemoveChild(view);
                    modified = true;
                }
            }
        }

        return (modified, FileName);
    }

    public Task CommitAsync()
    {
        if (FileName is null || !File.Exists(FileName))
        {
            return Task.CompletedTask;
        }

        if (doc is not null)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(FileName, settings);
            doc.Save(writer);
        }

        return Task.CompletedTask;
    }
}
