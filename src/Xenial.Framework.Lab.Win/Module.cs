using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xenial.Framework.Lab.Win
{
    [XenialCheckLicence]
    public sealed partial class XenialLabWindowsFormsModule : XenialModuleBase
    {
        /// <summary>   Gets required module types core. </summary>
        ///
        /// <returns>   The required module types core. </returns>

        protected override ModuleTypeList GetRequiredModuleTypesCore() => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(new[]
            {
                typeof(SystemWindowsFormsModule),
                typeof(XenialLabModule)
            });

        /// <summary>   returns empty types. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the declared controller types in this
        /// collection.
        /// </returns>
        ///
        /// <seealso cref="Xenial.Framework.XenialModuleBase.GetDeclaredControllerTypes()"/>

        protected override IEnumerable<Type> GetDeclaredControllerTypes() => base.GetDeclaredControllerTypes().Concat(new[]
        {
            typeof(ExtendedLayoutController)
        });
    }
}
