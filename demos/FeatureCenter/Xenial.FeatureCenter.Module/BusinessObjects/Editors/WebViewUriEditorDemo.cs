using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [Persistent]
    public partial class WebViewUriEditorDemo : FeatureCenterDemoBaseObjectId
    {
        private static readonly string[] schemes = new[] { "http://", "https://" };

        private string? urlString;
        private Uri? uri;

        public WebViewUriEditorDemo(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            UrlString = "https://www.xenial.io";
        }

        [ImmediatePostData]
        //This code converts for example 'www.xenial.io' to 'https://www.xenial.io' and sets the Uri
        //It's only relevant for the demo.
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

        [WebViewUriEditor]
        public Uri? Uri { get => uri; set => SetPropertyValue(ref uri, value); }
    }
}
