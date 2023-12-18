using System;
using System.Collections.Generic;
using System.Text;
using Xenial;
using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;
using System.Linq;

namespace MailClient.Module.BusinessObjects
{
    [XenialCollectExportedTypes]
    [XenialCollectControllers]
    internal static partial class TypeList
    {
        internal static IEnumerable<Type> UseBaseControllerTypes(this IEnumerable<Type> types)
            => types.Concat(ControllerTypes);
    }

    public class MailClientBuilderManager : XafBuilderManager
    {
        public MailClientBuilderManager(ITypesInfo typesInfo)
            : base(typesInfo) { }

        public MailClientBuilderManager(ITypesInfo typesInfo, IEnumerable<IBuilder> builders)
            : base(typesInfo, builders) { }

        protected override IEnumerable<IBuilder> GetBuilders() => new IBuilder[]
        {
            TypesInfo.CreateModelBuilder<MailSettingsModelBuilder>(),
            TypesInfo.CreateModelBuilder<MailAccountModelBuilder>(),
            TypesInfo.CreateModelBuilder<MailModelBuilder>(),
        };
    }
}
