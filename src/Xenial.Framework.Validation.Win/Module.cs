using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation.Win;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xenial.Framework.Validation.Win;

/// <summary>
/// 
/// </summary>
public sealed class XenialValidationWindowsFormsModule : XenialModuleBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override ModuleTypeList GetRequiredModuleTypesCore()
        => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(
                typeof(XenialValidationModule),
                typeof(ValidationWindowsFormsModule),
                typeof(SystemWindowsFormsModule)
            );
}

