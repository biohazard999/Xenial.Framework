using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    public class ModelBuilderIntroductionDemoBuilder : ModelBuilder<ModelBuilderIntroductionDemo>
    {
        public ModelBuilderIntroductionDemoBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.GenerateNoListViews();

            this.HasCaption("ModelBuilders - Introduction")
                .WithDefaultClassOptions()
                .HasImage("direction1")
                .IsSingleton(autoCommit: true)
            ;
        }
    }
}
