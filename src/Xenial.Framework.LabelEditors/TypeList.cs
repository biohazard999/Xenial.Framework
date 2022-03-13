using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.LabelEditors.Model;

namespace DevExpress.ExpressApp;

using Xenial.Framework.LabelEditors.Layout;
using Xenial.Framework.Model.GeneratorUpdaters;

/// <summary>
/// 
/// </summary>
public static partial class XenialLabelEditorsTypeList
{
    static XenialLabelEditorsTypeList() =>
        XenialDeeplinkModuleInitializer.Initialize();

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
