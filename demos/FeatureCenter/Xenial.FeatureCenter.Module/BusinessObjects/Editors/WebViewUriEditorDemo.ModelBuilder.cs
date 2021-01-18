using System;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;
using DevExpress.Persistent.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public class WebViewEditorDemoModelBuilder : FeatureCenterDemoBaseObjectIdModelBuilder<WebViewUriEditorDemo>
    {
        public WebViewEditorDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("Editors - WebViewUri")
                .WithDefaultClassOptions()
                .IsSingleton(autoCommit: true)
                .HasImage("Business_World");

            For(m => m.UrlString)
                .WithPredefinedValues(new[]
                {
                    "https://www.xenial.io",
                    "https://blog.xenial.io",
                    "https://www.devexpress.com",
                    "https://www.google.com",
                });

            this.WithDetailViewLayout(l => new()
            {
                BuildDemoLayout(l, (l, tab) => tab with
                {
                    Children = new(tab.Children)
                    {
                        l.TabbedGroup
                        (
                            l.Tab("Basic",
                                l.PropertyEditor(m => m.UrlString) with { CaptionLocation = Locations.Top },
                                l.PropertyEditor(m => m.Uri) with { ShowCaption = false, CaptionLocation = Locations.Top }
                            )
                        )
                    }
                })
            });
        }
    }
}
