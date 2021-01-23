using System;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    public class ModelBuilderIntroductionDemoBuilder : ModelBuilder<ModelBuilderIntroductionDemo>
    {
        public ModelBuilderIntroductionDemoBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("ModelBuilders - Introduction")
                .WithDefaultClassOptions()
                .HasImage("direction1")
            ;
        }
    }
}
