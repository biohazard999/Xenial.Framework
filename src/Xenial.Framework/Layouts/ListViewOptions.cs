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
}
