using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using DevExpress.ExpressApp.Model;

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
    private Action<IDetailViewOptionsExtensions>? extensions;

    /// <summary>
    /// 
    /// </summary>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IDetailViewOptionsExtensions ExtensionsCollection { get; } = new DetailViewOptionsExtensions();

    /// <summary>
    /// 
    /// </summary>
    public Action<IDetailViewOptionsExtensions>? Extensions
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
public sealed class GenericDetailViewOptions : IGenericDetailViewOptions
{
    private readonly Dictionary<string, object> dictionary = new();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object this[string index] { get => dictionary[index]; set => dictionary[index] = value; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<KeyValuePair<string, object>> AsEnumerable()
        => new ReadOnlyDictionary<string, object>(dictionary);
}

/// <summary>
/// 
/// </summary>
public interface IGenericDetailViewOptions : IDetailViewOptionsExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object this[string index]
    {
        get;
        set;
    }
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
    public static IDetailViewOptionsExtensions Generic(this IDetailViewOptionsExtensions list, GenericDetailViewOptions options)
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
public sealed class DetailViewOptionsExtensions : IDetailViewOptionsExtensions
{
    private readonly List<IDetailViewOptionsExtension> extensions = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extension"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(IDetailViewOptionsExtension extension)
        => extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="extensions"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddRange(IEnumerable<IDetailViewOptionsExtension> extensions)
    {
        _ = extensions ?? throw new ArgumentNullException(nameof(extensions));
        foreach (var extension in extensions)
        {
            Add(extension);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<IDetailViewOptionsExtension> AsEnumerable()
        => extensions.AsReadOnly();
}

/// <summary>
/// 
/// </summary>
public interface IDetailViewOptionsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="extension"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    void Add(IDetailViewOptionsExtension extension);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="extensions"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    void AddRange(IEnumerable<IDetailViewOptionsExtension> extensions);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    IEnumerable<IDetailViewOptionsExtension> AsEnumerable();
}

/// <summary>
/// 
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "By Design")]
public interface IDetailViewOptionsExtension { }
