using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

using Xenial;

namespace Xenial.Framework.Layouts;

/// <summary>
/// Defines options to create list views
/// </summary>
[XenialModelOptions(
    typeof(IModelListView), IgnoredMembers = new[]
    {
        nameof(IModelListView.Id),
        nameof(IModelListView.Index),
        nameof(IModelListView.ModelClass)
    }
)]
[XenialModelOptions(
    typeof(IModelAsync), IgnoredMembers = new[]
    {
        nameof(IModelListView.Id),
        nameof(IModelListView.Index)
    }
)]
[XenialModelOptions(typeof(IModelListViewNewItemRow))]
[XenialModelOptions(typeof(IModelListViewShowAutoFilterRow))]
[XenialModelOptions(typeof(IModelListViewShowFindPanel))]
public partial record ListViewOptions
{
    private bool automaticColumns;
    internal bool WasAutomaticColumnsSet { get; set; }
    /// <summary>
    /// TODO: Describe Automatic Columns
    /// </summary>
    public bool AutomaticColumns
    {
        get => automaticColumns;
        set
        {
            WasAutomaticColumnsSet = true;
            automaticColumns = value;
        }
    }

    private Action<ListViewOptionsExtensions>? extensions;

    /// <summary>
    /// 
    /// </summary>
    public ListViewOptionsExtensions ExtensionsCollection { get; private set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public Action<ListViewOptionsExtensions>? Extensions
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
[Serializable]
public sealed class GenericListViewOptions : Dictionary<string, object>, IListViewOptionsExtension
{
}

/// <summary>
/// 
/// </summary>
public static class ListViewOptionsExt
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ListViewOptionsExtensions Generic(this ListViewOptionsExtensions list, GenericListViewOptions options)
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
public sealed class ListViewOptionsExtensions : List<IListViewOptionsExtension> { }
/// <summary>
/// 
/// </summary>
public interface IListViewOptionsExtension { }
