using System;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public class StepProgressBarEnumEditorDemoModelBuilder : ModelBuilder<StepProgressBarEnumEditorDemo>
    {
        public StepProgressBarEnumEditorDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("Editors - StepProgressBarEnum")
                .WithDefaultClassOptions()
                .HasImage("GaugeStyleLinearHorizontal")
                .IsSingleton(autoCommit: true);

            this.WithDetailViewLayout(l => new()
            {
                l.TabbedGroup
                (
                    l.Tab("Demo", "Weather_Lightning") with
                    {
                        Children = new()
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
                    },
                    l.Tab("Installation", "ShipmentReceived") with
                    {
                        Children = new()
                        {
                            l.PropertyEditor(m => m.Installation),
                        }
                    },
                    l.Tab("Usage", "Actions_Settings") with
                    {
                        Children = new()
                        {
                            l.PropertyEditor(m => m.Usage),
                        }
                    },
                    l.Tab("Remarks", "Actions_Info") with
                    {

                    },
                    l.Tab("API-Docs", "DocumentStatistics") with
                    {

                    },
                    l.Tab("Demo-Code", "ToggleFieldCodes") with
                    {
                        Children = new()
                        {
                            l.PropertyEditor(m => m.DemoCode),
                        }
                    },
                    l.Tab("Supported Platforms", "Bool") with
                    {

                    }
                )
            }); ;
        }
    }
}
