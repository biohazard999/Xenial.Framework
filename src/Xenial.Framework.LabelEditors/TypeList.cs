using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xenial.Framework.LabelEditors.Model;

namespace DevExpress.ExpressApp;

/// <summary>
/// 
/// </summary>
public static partial class XenialLabelEditorsTypeList
{
    internal static IEnumerable<Type> RegularTypes { get; } = new[]
    {
        typeof(IHtmlContentViewItem)
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="regularTypes"></param>
    /// <returns></returns>
    public static IEnumerable<Type> UseXenialLabelEditorsRegularTypes(this IEnumerable<Type> regularTypes)
        => regularTypes.Concat(RegularTypes);
}
