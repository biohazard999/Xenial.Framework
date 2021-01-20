using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using System;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public class TokenStringEditorDemoModelBuilder : FeatureCenterEditorsBaseObjectModelBuilder<TokenStringEditorDemo>
    {
        public TokenStringEditorDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("Editors - TokenStringEditor")
               .WithDefaultClassOptions()
               .HasImage("AddQuery")
               .IsSingleton(autoCommit: true);

            this.WithDetailViewLayout(l => new()
            {
                BuildDemoLayout(l, (l, tab) => tab with
                {
                    Children = new(tab.Children)
                    {
                        l.TabbedGroup
                        (
                            l.Tab("Basic",
                                l.PropertyEditor(m => m.StringTokens) with { CaptionLocation = Locations.Top },
                                l.EmptySpaceItem() with { RelativeSize = 90 }
                            )
                        )
                    }
                })
            });
        }
    }
}
