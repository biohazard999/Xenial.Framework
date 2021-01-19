using DevExpress.Xpo;

using System;
using System.Collections.Generic;
using System.Text;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    public abstract class FeatureCenterModelBuildersBaseObject : FeatureCenterBaseObjectId
    {
        public FeatureCenterModelBuildersBaseObject(Session session) : base(session) { }

        public string Summary => BuildSummaryHtml();

        protected virtual string BuildSummaryHtml() => string.Empty;
    }
}
