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

    internal static class TypesInfoExtentions
    {
        public static TModelBuilder CreateModelBuilder<TModelBuilder>(this ITypesInfo typesInfo)
        {
            var builderForType = typeof(TModelBuilder).BaseType.GenericTypeArguments.FirstOrDefault();
            if (builderForType == null)
            {
                throw new InvalidOperationException($"Cannot create ModelBuilder of Type '{typeof(TModelBuilder)}' because the base generic type can not be found");
            }
            var typeInfo = typesInfo.FindTypeInfo(builderForType);
            if (typeInfo == null)
            {
                throw new InvalidOperationException($"Cannot create ModelBuilder of Type '{typeof(TModelBuilder)}' because no TypeInfo for Type '{builderForType}' could be found");
            }
            var builder = Activator.CreateInstance(typeof(TModelBuilder), typeInfo);
            return (TModelBuilder)builder;
        }
    }
}
