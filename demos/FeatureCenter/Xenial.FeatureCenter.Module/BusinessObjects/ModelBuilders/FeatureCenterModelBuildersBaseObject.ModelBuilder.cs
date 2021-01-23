using DevExpress.ExpressApp.DC;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    public class FeatureCenterModelBuildersBaseObjectModelBuilder : ModelBuilder<FeatureCenterModelBuildersBaseObject>
    {
        public FeatureCenterModelBuildersBaseObjectModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.GenerateNoViews();

            For(m => m.Introduction)
                .UseWebViewHtmlStringPropertyEditor();
        }
    }
}
