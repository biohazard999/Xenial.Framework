using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Layouts;

/// <summary>
/// 
/// </summary>
public static class WinDetailViewOptionsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DetailViewOptionsExtensions WindowsForms(this DetailViewOptionsExtensions list, DetailViewOptionsWin options)
    {
        _ = list ?? throw new ArgumentNullException(nameof(list));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        list.Add(options);

        return list;
    }
}

internal static class XenialWindowsFormsModuleInitializer
{
#if NET5_0_OR_GREATER
    [System.Runtime.CompilerServices.ModuleInitializer]
#endif
    internal static void Initialize()
        => MappingFactory
            .RegisterDetailOptionsMapper((options, model) =>
            {
                if (options is DetailViewOptionsWin winOptions)
                {
                    new ViewOptionsMapper()
                        .Map(winOptions, model);
                }
            });
}
