using System;
using System.Linq;

using DevExpress.Xpo;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    [Persistent]
    public partial class ModelBuilderIntroductionDemo : FeatureCenterModelBuildersBaseObject
    {
        public ModelBuilderIntroductionDemo(Session session) : base(session) { }
    }
}
