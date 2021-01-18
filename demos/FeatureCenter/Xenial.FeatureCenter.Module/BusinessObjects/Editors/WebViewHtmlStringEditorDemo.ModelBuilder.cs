using System;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;
using DevExpress.Persistent.Base;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public class WebViewHtmlStringEditorDemoModelBuilder : FeatureCenterDemoBaseObjectIdModelBuilder<WebViewHtmlStringEditorDemo>
    {
        public WebViewHtmlStringEditorDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("Editors - WebViewHtmlStringEditor")
                .WithDefaultClassOptions()
                .IsSingleton(autoCommit: true)
                .HasImage("Business_World");

            this.WithDetailViewLayout(l => new()
            {
                BuildDemoLayout(l, (l, tab) => tab with
                {
                    Children = new(tab.Children)
                    {
                        l.TabbedGroup
                        (
                            l.Tab("Basic",
                                l.PropertyEditor(m => m.HtmlContent) with { CaptionLocation = Locations.Top },
                                l.PropertyEditor(m => m.Html) with { ShowCaption = false, CaptionLocation = Locations.Top }
                            )
                        )
                    }
                })
            });
        }
    }
}
