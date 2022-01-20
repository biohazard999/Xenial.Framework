using System;
using System.Collections.Generic;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

namespace Xenial.Framework.Layouts;

/// <summary>
/// Defines options to create detail views
/// </summary>
[XenialModelOptions(
    typeof(IModelDetailView), IgnoredMembers = new[]
    {
        nameof(IModelDetailView.Id),
        nameof(IModelDetailView.Index),
        nameof(IModelDetailView.ModelClass)
    }
)]
[XenialModelOptions(
    typeof(IModelAsync), IgnoredMembers = new[]
    {
        nameof(IModelListView.Id),
        nameof(IModelListView.Index)
    }
)]
//TODO: Default Focused Item
//[XenialModelOptions(
//    typeof(IModelDetailViewDefaultFocusedItem), IgnoredMembers = new[]
//    {
//        nameof(IModelWinLayoutManagerOptions.Index)
//    }
//)]
public partial record DetailViewOptions
{
    private Action<DetailViewOptionsExtensions>? extensions;

    /// <summary>
    /// 
    /// </summary>
    public DetailViewOptionsExtensions ExtensionsCollection { get; private set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public Action<DetailViewOptionsExtensions>? Extensions
    {
        get => extensions;
        set
        {
            extensions = value;
            value?.Invoke(ExtensionsCollection);
        }
    }
}

/// <summary>
/// 
/// </summary>
public sealed class GenericDetailViewOptions : Dictionary<string, object>, IDetailViewOptionsExtension
{
}

/// <summary>
/// 
/// </summary>
public static class DetailViewOptionsExt
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DetailViewOptionsExtensions Generic(this DetailViewOptionsExtensions list, GenericDetailViewOptions options)
    {
        _ = list ?? throw new ArgumentNullException(nameof(list));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        list.Add(options);

        return list;
    }
}

/// <summary>
/// 
/// </summary>
public sealed class DetailViewOptionsExtensions : List<IDetailViewOptionsExtension> { }

/// <summary>
/// 
/// </summary>
public interface IDetailViewOptionsExtension { }
