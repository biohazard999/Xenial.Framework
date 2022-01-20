using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts;

namespace Xenial.Framework.Model.GeneratorUpdaters;

public sealed class MappingFactory
{
    internal static MappingFactory Factory { get; } = new();

    private readonly List<MapDetailViewOptionsExtension> detailOptionMappers = new();

    public static void RegisterDetailOptionsMapper(MapDetailViewOptionsExtension mapper)
    {
        _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Factory.detailOptionMappers.Add(mapper);
    }

    internal void MapDetailViewOptions(IDetailViewOptionsExtension extension, IModelNode node)
    {
        foreach (var detailOptionMapper in detailOptionMappers)
        {
            detailOptionMapper(extension, node);
        }
    }
}

public delegate void MapDetailViewOptionsExtension(IDetailViewOptionsExtension extension, IModelNode node);
