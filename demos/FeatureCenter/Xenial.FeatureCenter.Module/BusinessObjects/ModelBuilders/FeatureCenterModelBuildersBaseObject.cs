using System;

using DevExpress.Xpo;

using static Xenial.FeatureCenter.Module.HtmlBuilders.HtmlBuilder;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    public abstract class FeatureCenterModelBuildersBaseObject : FeatureCenterBaseObjectId
    {
        public FeatureCenterModelBuildersBaseObject(Session session) : base(session) { }

        public string Introduction => BuildHtml("Introduction", BuildIntroductionHtml());

        protected virtual string BuildIntroductionHtml()
            => MarkDownBlock.FromResourceString("BusinessObjects/ModelBuilders/ModelBuilderIntroductionDemo.Introduction.md").ToString();
    }
}
