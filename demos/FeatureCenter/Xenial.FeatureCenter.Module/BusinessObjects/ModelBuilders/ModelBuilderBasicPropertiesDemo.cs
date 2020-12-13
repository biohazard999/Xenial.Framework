using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    [DomainComponent]
    public class ModelBuilderBasicPropertiesDemo : NonPersistentBaseObject
    {
        private static readonly string[] schemes = new[] { "http://", "https://" };

        private string? demoCode = "https://www.xenial.io";
        [ImmediatePostData]
        public string? DemoCode
        {
            get => demoCode;
            set
            {
                if (SetPropertyValue(ref demoCode, value))
                {
                    if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var absoluteOrRelativeUri))
                    {
                        if (absoluteOrRelativeUri.IsAbsoluteUri)
                        {
                            DemoUri = absoluteOrRelativeUri;
                            return;
                        }

                        if (!string.IsNullOrEmpty(value)
                            && schemes.Any(scheme => value?.StartsWith(scheme, StringComparison.InvariantCultureIgnoreCase) != true)
                        )
                        {
                            value = $"https://{value}";
                            if (Uri.TryCreate(value, UriKind.Absolute, out var absoluteUri))
                            {
                                DemoUri = absoluteUri;
                            }
                        }
                    }
                }
            }
        }

        private Uri? demoUri = new Uri("https://www.xenial.io");
        [EditorAlias("WebViewUriPropertyEditor")]
        public Uri? DemoUri { get => demoUri; set => SetPropertyValue(ref demoUri, value); }

    }
}
