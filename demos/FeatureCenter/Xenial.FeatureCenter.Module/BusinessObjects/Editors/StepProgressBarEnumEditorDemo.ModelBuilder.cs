using System;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public class StepProgressBarEnumEditorDemoModelBuilder : FeatureCenterEditorsBaseObjectModelBuilder<StepProgressBarEnumEditorDemo>
    {
        public StepProgressBarEnumEditorDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("Editors - StepProgressEnumEditor")
                .WithDefaultClassOptions()
                .HasImage("GaugeStyleLinearHorizontal")
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
                                l.PropertyEditor(m => m.NormalSteps) with { CaptionLocation = Locations.Top },
                                l.LayoutGroup("Normal Enumeration", l.PropertyEditor(m => m.Steps)) with { RelativeSize = 33 },
                                l.LayoutGroup("Nullable Enumeration", l.PropertyEditor(m => m.NullableSteps)) with { RelativeSize = 33 },
                                l.EmptySpaceItem() with { RelativeSize = 33 }
                            ),
                            l.Tab("Display Styles",
                                l.LayoutGroup("Without Description", l.PropertyEditor(m => m.WithoutDescription)) with { RelativeSize = 33 },
                                l.LayoutGroup("Without Images", l.PropertyEditor(m => m.WithoutImages)) with { RelativeSize = 33 },
                                l.LayoutGroup("Caption Only", l.PropertyEditor(m => m.CaptionOnly)) with { RelativeSize = 33 },
                                l.EmptySpaceItem() with { RelativeSize = 1 }
                            )
                        )
                    }
                })
            });
        }
    }
}
