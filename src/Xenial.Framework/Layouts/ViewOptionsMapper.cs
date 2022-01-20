using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

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
}
