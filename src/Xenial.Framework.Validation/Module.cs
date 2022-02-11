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
public static class XenialValidationModuleInitializer
{
    /// <summary>
    /// 
    /// </summary>
#if NET5_0_OR_GREATER
    [ModuleInitializer]
#endif
    public static void Initialize()
        => MappingFactory
            .RegisterDetailOptionsMapper((options, model) =>
            {
                if (options is ValidationDetailViewOptions validationOptions)
                {
                    new ValidationViewOptionsMapper()
                        .Map(validationOptions, model);
                }
            });
}
