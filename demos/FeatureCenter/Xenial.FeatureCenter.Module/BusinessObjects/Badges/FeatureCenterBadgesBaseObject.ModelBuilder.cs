using System;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Badges
{
    public sealed class FeatureCenterBadgesBaseObjectModelBuilder : ModelBuilder<FeatureCenterBadgesBaseObject>
    {
        public FeatureCenterBadgesBaseObjectModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.GenerateNoViews();

            For(m => m.Introduction)
                .UseWebViewHtmlStringPropertyEditor();

            For(m => m.Installation)
                .UseWebViewHtmlStringPropertyEditor();
        }
    }
}
