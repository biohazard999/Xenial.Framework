using System;
using System.Collections.Generic;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;

using MailClient.Module.BusinessObjects;

using Xenial.Framework;
using Xenial.Framework.ModelBuilders;

namespace MailClient.Module
{
    public class MailClientModule : XenialModuleBase
    {
        protected override IEnumerable<Type> GetDeclaredExportedTypes() => ModelTypeList.PersistentTypes;

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            typesInfo.RemoveXafViewsFromApplicationModel();
            typesInfo.RemoveXpoViewsFromApplicationModel();

            ModelBuilder.Create<MailBaseObject>(typesInfo).GenerateNoViews().Build();
            ModelBuilder.Create<MailBaseObjectId>(typesInfo).GenerateNoViews().Build();
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.UseNoViewsGeneratorUpdater();
        }
    }
}
