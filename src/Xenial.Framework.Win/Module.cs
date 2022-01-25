using System;

using Xenial.Framework.Layouts;
using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Win;

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
