using System;
using System.Collections.Generic;

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

        public string Installation => BuildHtml("Installation", BuildInstallationHtml());

        protected virtual string BuildInstallationHtml()
            => NugetInstallSection(GetRequiredModules()).ToString();

        protected virtual IEnumerable<RequiredNuget> GetRequiredModules() => new[]
        {
            new RequiredNuget("Xenial.Framework")
        };
    }
}
