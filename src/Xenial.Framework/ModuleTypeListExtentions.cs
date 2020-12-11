using System;

namespace DevExpress.ExpressApp
{
    /// <summary>
    /// Provides Extention Methods for <see cref="ModuleTypeList"/>
    /// </summary>
    public static class ModuleTypeListExtentions
    {
        /// <summary>
        /// Adds types to the <see cref="ModuleTypeList"/>
        /// </summary>
        /// <param name="moduleTypeList">The module type list.</param>
        /// <param name="types">The types.</param>
        /// <returns>The original <see cref="ModuleTypeList"/></returns>
        public static ModuleTypeList AndModuleTypes(this ModuleTypeList moduleTypeList, params Type[] types)
        {
            _ = moduleTypeList ?? throw new ArgumentNullException(nameof(moduleTypeList));
            moduleTypeList.AddRange(types);
            return moduleTypeList;
        }
    }
}
