using System.Runtime.CompilerServices;

using Xenial.Framework.Layouts;
using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Validation;

/// <summary>
/// 
/// </summary>
public sealed class XenialValidationModule : XenialModuleBase
{
    static XenialValidationModule()
        => XenialValidationModuleInitializer.Initialize();
}

/// <summary>
/// 
/// </summary>
#if NET5_0_OR_GREATER
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
public static class XenialValidationModuleInitializer
{
    private static bool initialized;
    /// <summary>
    /// 
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.CompilerServices.ModuleInitializer]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        MappingFactory
            .RegisterDetailOptionsMapper((options, model) =>
            {
                if (options is ValidationDetailViewOptions validationOptions)
                {
                    new ValidationViewOptionsMapper()
                        .Map(validationOptions, model);
                }
            });
    }
}
