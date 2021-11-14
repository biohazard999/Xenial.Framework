﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xenial.Framework.StepProgressEditors.Win
{
    /// <summary>
    /// Class XenialStepProgressEditorsWindowsFormsModule.
    /// Implements the <see cref="Xenial.Framework.XenialModuleBase" />
    /// </summary>
    /// <seealso cref="Xenial.Framework.XenialModuleBase" />
    /// <autogeneratedoc />
    [XenialCheckLicense]
    public sealed partial class XenialStepProgressEditorsWindowsFormsModule : XenialModuleBase
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
                    typeof(SystemWindowsFormsModule),
                    typeof(XenialStepProgressEditorsModule)
                });

        /// <summary>   Registers the editor descriptors. </summary>
        ///
        /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.UseStepProgressEnumPropertyEditorsWin();
        }

#if DX_LTE_20_2_4
        /// <summary>
        /// Registers the Generator Updaters. These are classes, used to customize the Application Model's zero layer after it has been generated.
        /// </summary>
        /// <param name="updaters">A ModelNodesGeneratorUpdaters object providing access to the list of Generator Updaters.</param>
        /// <autogeneratedoc />
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.UseStepProgressEnumPropertyEditors();
        }
#endif
    }
}
