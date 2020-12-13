using System;
using System.Collections.Generic;
using System.Linq;
using Xenial.FeatureCenter.Module.BusinessObjects.Editors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;

using Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders;
using Xenial.Framework;
using Xenial.Framework.Base;
using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module
{
    public class FeatureCenterModule : XenialModuleBase
    {
        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
            .Concat(new[]
            {
                typeof(ModelBuilderBasicPropertiesDemo),
                typeof(WebViewEditorDemo)
            });

        protected override IEnumerable<Type> GetDeclaredControllerTypes()
            => base.GetDeclaredControllerTypes()
                .UseSingletonController();

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            updaters.UseSingletonNavigationItems();
            base.AddGeneratorUpdaters(updaters);
        }

        public override void Setup(XafApplication application)
        {
            application.UseNonPersistentSingletons();
            base.Setup(application);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            typesInfo.CreateModelBuilder<ModelBuilderBasicPropertiesDemoModelBuilder>().Build();
        }
    }
}
