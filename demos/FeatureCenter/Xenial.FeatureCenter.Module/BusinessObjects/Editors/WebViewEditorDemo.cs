using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [DomainComponent]
    [Singleton]
    [DefaultClassOptions]
    [ImageName("globe")]
    public class WebViewEditorDemo : NonPersistentBaseObject
    {
        private static readonly string[] schemes = new[] { "http://", "https://" };

        private string? urlString = "https://www.xenial.io";
        [ImmediatePostData]
        public string? UrlString
        {
            get => urlString;
            set
            {
                if (SetPropertyValue(ref urlString, value))
                {
                    if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var absoluteOrRelativeUri))
                    {
                        if (absoluteOrRelativeUri.IsAbsoluteUri)
                        {
                            Uri = absoluteOrRelativeUri;
                            return;
                        }

                        if (!string.IsNullOrEmpty(value)
                            && schemes.Any(scheme => value?.StartsWith(scheme, StringComparison.InvariantCultureIgnoreCase) != true)
                        )
                        {
                            value = $"https://{value}";
                            if (Uri.TryCreate(value, UriKind.Absolute, out var absoluteUri))
                            {
                                Uri = absoluteUri;
                            }
                        }
                    }
                }
            }
        }

        private Uri? uri = new Uri("about:blank");
        [EditorAlias("WebViewUriPropertyEditor")]
        public Uri? Uri { get => uri; set => SetPropertyValue(ref uri, value); }

    }
}
