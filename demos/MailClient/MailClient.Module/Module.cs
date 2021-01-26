using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;

using MailClient.Module.BusinessObjects;
using MailClient.Module.Updaters;

using Xenial.Framework;
using Xenial.Framework.ModelBuilders;

namespace MailClient.Module
{
    public class MailClientModule : XenialModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore() => base.GetRequiredModuleTypesCore().AndModuleTypes(new[]
        {
            typeof(ValidationModule)
        });

        protected override IEnumerable<Type> GetDeclaredExportedTypes() => ModelTypeList.PersistentTypes;

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => new[]
            {
                typeof(Xenial.Framework.SystemModule.SingletonController), //TODO: Extension method for feature slice
                typeof(ReceiveMailsViewController)
            };

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) => base.GetModuleUpdaters(objectSpace, versionFromDB).Concat(new ModuleUpdater[]
        {
            new MailClientSeedModelUpdater(objectSpace, versionFromDB)
        });

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.UseTokenStringPropertyEditors();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            typesInfo.RemoveXafViewsFromApplicationModel();
            typesInfo.RemoveXpoViewsFromApplicationModel();

            ModelBuilder.Create<MailBaseObject>(typesInfo).GenerateNoViews().Build();
            ModelBuilder.Create<MailBaseObjectId>(typesInfo).GenerateNoViews().Build();

            new MailClientBuilderManager(typesInfo)
                .Build();
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.UseApplicationOptions(new Xenial.Framework.Model.GeneratorUpdaters.ApplicationOptions
            {
                Layout =
                {
                    CaptionLocation = DevExpress.Persistent.Base.Locations.Top
                }
            });

            updaters.UseSingletonNavigationItems();
            updaters.UseNoViewsGeneratorUpdater();
        }
    }
}
