using System;
using System.Collections.Generic;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts;

namespace Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// 
/// </summary>
public sealed class MappingFactory
{
    static MappingFactory()
    {
        RegisterDetailOptionsMapper((options, node) =>
        {
            if (options is GenericDetailViewOptions genericOptions)
            {
                ViewOptionsMapper.MapGenericOptions(genericOptions, node);
            }
        });

        RegisterDetailOptionsMapper((options, node) =>
        {
            if (options is HiddenActionsOptions hiddenActionsOptions)
            {
                ViewOptionsMapper.MapHiddenActions(hiddenActionsOptions, node);
            }
        });

        RegisterListOptionsMapper((options, node) =>
        {
            if (options is GenericListViewOptions genericOptions)
            {
                ViewOptionsMapper.MapGenericOptions(genericOptions, node);
            }
        });
    }

    internal static MappingFactory Factory { get; } = new();

    private readonly List<MapDetailViewOptionsExtension> detailOptionMappers = new();
    private readonly List<MapListViewOptionsExtension> listOptionMappers = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterDetailOptionsMapper(MapDetailViewOptionsExtension mapper)
    {
        _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Factory.detailOptionMappers.Add(mapper);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterListOptionsMapper(MapListViewOptionsExtension mapper)
    {
        _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Factory.listOptionMappers.Add(mapper);
    }

    internal void MapDetailViewOptions(IDetailViewOptionsExtension extension, IModelNode node)
    {
        foreach (var detailOptionMapper in detailOptionMappers)
        {
            detailOptionMapper(extension, node);
        }
    }


    internal void MapListViewOptions(IListViewOptionsExtension extension, IModelNode node)
    {
        foreach (var listOptionMapper in listOptionMappers)
        {
            listOptionMapper(extension, node);
        }
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="extension"></param>
/// <param name="node"></param>
public delegate void MapDetailViewOptionsExtension(IDetailViewOptionsExtension extension, IModelNode node);

/// <summary>
/// 
/// </summary>
/// <param name="extension"></param>
/// <param name="node"></param>
public delegate void MapListViewOptionsExtension(IListViewOptionsExtension extension, IModelNode node);
