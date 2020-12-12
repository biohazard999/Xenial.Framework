using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    [DomainComponent]
    [Singleton]
    [DefaultClassOptions]
    [ImageName("direction1")]
    public class ModelBuilderBasicPropertiesDemo : NonPersistentBaseObject
    {
        private Uri? demoUri = new Uri("https://www.xenial.io");
        [EditorAlias("WebViewUriPropertyEditor")]
        public Uri? DemoUri { get => demoUri; set => SetPropertyValue(ref demoUri, value); }

        private string? demoCode;
        public string? DemoCode { get => demoCode; set => SetPropertyValue(ref demoCode, value); }
    }
}
