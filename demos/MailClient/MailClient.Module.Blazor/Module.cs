using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Validation.Blazor;

using Xenial.Framework;

namespace MailClient.Module.Blazor
{
    public class MailClientBlazorModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore().AndModuleTypes(new[]
            {
                typeof(ValidationBlazorModule),
                typeof(MailClientModule)
            });

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.UseTokenStringPropertyEditorsBlazor();
        }
    }
}
