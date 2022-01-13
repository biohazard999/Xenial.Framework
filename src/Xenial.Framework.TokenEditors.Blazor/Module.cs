﻿using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.Editors;


namespace Xenial.Framework.TokenEditors.Blazor
{
    /// <summary>
    /// Class XenialTokenEditorsBlazorModule.
    /// Implements the <see cref="Xenial.Framework.XenialModuleBase" />
    /// </summary>
    /// <seealso cref="Xenial.Framework.XenialModuleBase" />
    /// <autogeneratedoc />
    [XenialCheckLicense]
    public sealed partial class XenialTokenEditorsBlazorModule : XenialModuleBase
    {
        /// <summary>
        /// Adds the DevExpress.ExpressApp.SystemModule.SystemModule to the collection.
        /// </summary>
        ///
        /// <returns>   ModuleTypeList. </returns>

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(new[]
                {
                    typeof(SystemBlazorModule),
                    typeof(XenialTokenEditorsModule)
                });

        /// <summary>   Registers the editor descriptors. </summary>
        ///
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.UseTokenStringPropertyEditorsBlazor();
            editorDescriptorsFactory.UseTokenObjectsPropertyEditorsBlazor();
        }
    }
}
