using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

namespace MailClient.Module.BusinessObjects
{
    internal static class ModelTypeList
    {
        internal static readonly Type[] PersistentTypes = new[]
        {
            typeof(MailBaseObject),
            typeof(MailBaseObjectId),

            typeof(MailSettings),
        };
    }

    public class MailClientBuilderManager : XafBuilderManager
    {
        public MailClientBuilderManager(ITypesInfo typesInfo)
            : base(typesInfo) { }

        public MailClientBuilderManager(ITypesInfo typesInfo, IEnumerable<IBuilder> builders)
            : base(typesInfo, builders) { }

        protected override IEnumerable<IBuilder> GetBuilders() => new IBuilder[]
        {
            TypesInfo.CreateModelBuilder<MailSettingsModelBuilder>()
        };
    }
}
