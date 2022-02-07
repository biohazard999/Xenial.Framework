using System;
using System.Collections;
using System.Collections.Generic;
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
        => extensions;
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
    IEnumerable<IDetailViewOptionsExtension> AsEnumerable()

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    bool Equals(object obj);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    int GetHashCode();

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    string ToString();

    /// <summary>Gets the <see cref="System.Type"/> of the current instance.</summary>
    /// <returns>The <see cref="System.Type"/> instance that represents the exact runtime
    /// type of the current instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "By Design")]
    Type GetType();
}

/// <summary>
/// 
/// </summary>
public interface IDetailViewOptionsExtension
{
    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    bool Equals(object obj);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    int GetHashCode();

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    string ToString();

    /// <summary>Gets the <see cref="System.Type"/> of the current instance.</summary>
    /// <returns>The <see cref="System.Type"/> instance that represents the exact runtime
    /// type of the current instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "By Design")]
    Type GetType();
}

