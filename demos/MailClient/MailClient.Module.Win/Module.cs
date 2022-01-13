using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Validation.Win;

using Xenial.Framework;

namespace MailClient.Module.Win
{
    public class MailClientWindowsFormsModule : XenialModuleBase
    {
        public MailClientWindowsFormsModule() : base(useNullDiffsStore: false) { }

        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore().AndModuleTypes(new[]
            {
                typeof(ValidationWindowsFormsModule),
                typeof(MailClientModule)
            });

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => base.GetDeclaredControllerTypes()
                .UseXenialWindowsFormsControllers()
                .Concat(new[]
                {
                    typeof(CustomHRController)
                });

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory
                .UseTokenStringPropertyEditorsWin()
                .UseLabelStringPropertyEditorsWin();
        }
    }
}
