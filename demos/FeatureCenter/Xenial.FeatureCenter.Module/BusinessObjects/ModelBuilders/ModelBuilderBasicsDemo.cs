using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    [Persistent]
    public class ModelBuilderBasicsDemo : FeatureCenterModelBuildersBaseObject
    {
        public ModelBuilderBasicsDemo(Session session) : base(session) { }
    }
}
