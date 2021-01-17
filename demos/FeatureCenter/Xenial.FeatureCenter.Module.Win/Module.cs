using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Xpo;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using Xenial.Framework;
using Xenial.Framework.StepProgressEditors.Win;
using Xenial.Framework.TokenEditors.Win;
using Xenial.Framework.WebView.Win;

namespace Xenial.FeatureCenter.Module.Win
{
    public class FeatureCenterWindowsFormsModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(
                typeof(FeatureCenterModule),
                typeof(XenialWebViewWindowsFormsModule),

                typeof(XenialTokenEditorsWindowsFormsModule),
                typeof(XenialStepProgressEditorsWindowsFormsModule)
            );

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);

            editorDescriptorsFactory.UseTokenObjectsPropertyEditorsWin<TokenEditorNonPersistentTokens>();
            //editorDescriptorsFactory.UseTokenObjectsPropertyEditorsForTypeWin<XPCollection<TokenEditorPersistentTokens>>();

            editorDescriptorsFactory.RegisterPropertyEditor(
                "Xenial.WebViewStringPropertyEditor",
                typeof(string),
                typeof(WebViewStringPropertyEditor),
                false
            );
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.UseStepProgressEnumPropertyEditors();
        }
    }
}
