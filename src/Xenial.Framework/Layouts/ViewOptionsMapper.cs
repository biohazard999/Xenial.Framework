using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Layouts;

[XenialModelOptionsMapper(typeof(DetailViewOptions))]
[XenialModelOptionsMapper(typeof(ListViewOptions))]
internal partial class ViewOptionsMapper
{
    partial void MapNodeCore(DetailViewOptions from, IModelDetailView to)
    {
        foreach (var option in from.ExtensionsCollection)
        {
            MappingFactory.Factory.MapDetailViewOptions(option, to);
        }
    }

    public static void MapGenericOptions(GenericDetailViewOptions genericOptions, IModelNode node)
    {
        if (node is ModelNode modelNode)
        {
            foreach (var item in genericOptions)
            {
                modelNode.SetValue(item.Key, item.Value);
            }
        }
    }
}
