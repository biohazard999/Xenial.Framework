using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using System;
using System.Linq;

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

            For(m => m.TokenPopupFilterModeContainsStringTokens)
                .WithPredefinedValues(TokenStringEditorDemo.DemoTokens.ToArray())
                .HasCaption(nameof(TokenPopupFilterMode.Contains));

            For(m => m.TokenPopupFilterModeStartsWithStringTokens)
                .WithPredefinedValues(TokenStringEditorDemo.DemoTokens.ToArray())
                .HasCaption(nameof(TokenPopupFilterMode.StartsWith));

            For(m => m.DropDownShowModeOutlookStringTokens)
                .WithPredefinedValues(TokenStringEditorDemo.DemoTokens.ToArray())
                .HasCaption(nameof(TokenDropDownShowMode.Outlook));

            For(m => m.DropDownShowModeRegularStringTokens)
                .WithPredefinedValues(TokenStringEditorDemo.DemoTokens.ToArray())
                .HasCaption(nameof(TokenDropDownShowMode.Regular));

            this.WithDetailViewLayout(l => new()
            {
                BuildDemoLayout(l, (l, tab) => tab with
                {
                    Children = new(tab.Children)
                    {
                        l.TabbedGroup
                        (
                            l.Tab("Basic",
                                l.PropertyEditor(m => m.TokensWithoutPredefinedValues) with { CaptionLocation = Locations.Top },
                                l.PropertyEditor(m => m.StringTokens) with { CaptionLocation = Locations.Top },
                                l.PropertyEditor(m => m.AllowUserDefinedStringTokens) with { CaptionLocation = Locations.Top },
                                l.EmptySpaceItem() with { RelativeSize = 90 }
                            ),
                            l.Tab("Display Style",
                                l.VerticalGroup("DropDownShowMode",
                                    l.PropertyEditor(m => m.DropDownShowModeOutlookStringTokens) with { CaptionLocation = Locations.Top },
                                    l.PropertyEditor(m => m.DropDownShowModeRegularStringTokens) with { CaptionLocation = Locations.Top }
                                ),
                                l.VerticalGroup("PopupFilterMode",
                                    l.PropertyEditor(m => m.TokenPopupFilterModeContainsStringTokens) with { CaptionLocation = Locations.Top },
                                    l.PropertyEditor(m => m.TokenPopupFilterModeStartsWithStringTokens) with { CaptionLocation = Locations.Top }
                                ),
                                l.EmptySpaceItem() with { RelativeSize = 90 }
                            )
                        )
                    }
                })
            });
        }
    }
}
