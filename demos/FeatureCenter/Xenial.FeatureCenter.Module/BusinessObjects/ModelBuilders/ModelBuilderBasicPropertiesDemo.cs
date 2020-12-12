using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    [DomainComponent]
    public class ModelBuilderBasicPropertiesDemo : DevExpress.ExpressApp.NonPersistentBaseObject
    {
        private string? demoCode;
        public string? DemoCode { get => demoCode; set => SetPropertyValue(ref demoCode, value); }
    }
}
