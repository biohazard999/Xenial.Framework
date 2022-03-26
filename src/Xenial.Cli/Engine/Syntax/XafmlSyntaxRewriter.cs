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
    public FileModelStore? ModelStore { get; }
    public IList<string> RemovedViews { get; }

    public XafmlSyntaxRewriter(FileModelStore? modelStore, IList<string> removedViews!!)
    {
        ModelStore = modelStore;
        RemovedViews = removedViews;
    }

    private XmlDocument? doc;

    public async Task<(bool hasModifications, string? modelFilePath)> RewriteAsync()
    {
        if (ModelStore is null)
        {
            return (false, null);
        }

        var content = await File.ReadAllTextAsync(ModelStore.Name);
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

        return (modified, ModelStore.Name);
    }

    public Task CommitAsync()
    {
        if (ModelStore is null)
        {
            return Task.CompletedTask;
        }

        if (doc is not null)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(ModelStore.Name, settings);
            doc.Save(writer);
        }

        return Task.CompletedTask;
    }
}
