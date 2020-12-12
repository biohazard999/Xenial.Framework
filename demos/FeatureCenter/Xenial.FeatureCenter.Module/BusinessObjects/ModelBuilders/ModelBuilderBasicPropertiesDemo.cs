﻿using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    [DomainComponent]
    [Singleton]
    [DefaultClassOptions]
    public class ModelBuilderBasicPropertiesDemo : DevExpress.ExpressApp.NonPersistentBaseObject
    {
        private string? demoCode;
        public string? DemoCode { get => demoCode; set => SetPropertyValue(ref demoCode, value); }
    }
}
