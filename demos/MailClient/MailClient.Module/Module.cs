using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;

using MailClient.Module.BusinessObjects;
using MailClient.Module.Updaters;

using Xenial.Framework;
using Xenial.Framework.DevTools;
using Xenial.Framework.ModelBuilders;

namespace MailClient.Module
{
    public class MailClientModule : XenialModuleBase
    {
        public MailClientModule() : base(useNullDiffsStore: false) { }

        protected override ModuleTypeList GetRequiredModuleTypesCore() => base.GetRequiredModuleTypesCore().AndModuleTypes(new[]
        {
            typeof(ValidationModule),
            typeof(XenialDevToolsModule)
        });

        protected override IEnumerable<Type> GetRegularTypes() => base.GetRegularTypes()
            .UseXenialTokenStringEditorRegularTypes();

        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => TypeList.ExportedTypes;

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => new[]
            {
                typeof(Xenial.Framework.SystemModule.XenialSingletonViewController), //TODO: Extension method for feature slice
            }.UseBaseControllerTypes();

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) => base.GetModuleUpdaters(objectSpace, versionFromDB).Concat(new ModuleUpdater[]
        {
            new MailClientSeedModelUpdater(objectSpace, versionFromDB)
        });

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory
                .UseTokenStringPropertyEditors()
                .UseLabelStringPropertyEditors();
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);

            extenders
                .UseTokenStringPropertyEditors();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            typesInfo
                .RemoveXafViewsFromApplicationModel()
                .RemoveXpoViewsFromApplicationModel();

            var builder = ModelBuilder.Create<MailBaseObject>(typesInfo);

            builder.For(m => m.IsDeleted)
                .HasTooltip("");

            builder.Build();

            ModelBuilder.Create<MailBaseObject>(typesInfo).GenerateNoViews().Build();
            ModelBuilder.Create<MailBaseObjectId>(typesInfo).GenerateNoViews().Build();

            new MailClientBuilderManager(typesInfo)
                .Build();
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.UseApplicationOptions(new()
            {
                Layout =
                {
                    CaptionLocation = Locations.Top
                }
            });

            updaters.UseXenialImages();
            updaters.UseSingletonNavigationItems();
            updaters.UseNoViewsGeneratorUpdater();
            updaters.UseDeclareViewsGeneratorUpdater();
            updaters.UseDetailViewLayoutBuilders();
            updaters.UseListViewColumnBuilders();
        }
    }
}
