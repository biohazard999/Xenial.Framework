using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public class StepProgressBarEnumEditorDemoModelBuilder : ModelBuilder<StepProgressBarEnumEditorPersistentDemo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepProgressBarEnumEditorDemoModelBuilder`1"/> class.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        public StepProgressBarEnumEditorDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

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
                            l.PropertyEditor(m => m.InstallationHtml),
                        }
                    },
                    l.Tab("Usage", "Actions_Settings") with
                    {
                        Children = new()
                        {
                            l.PropertyEditor(m => m.InstallationMarkdown),
                        }
                    },
                    l.Tab("Remarks", "Actions_Info") with
                    {

                    },
                    l.Tab("Documentation", "DocumentStatistics") with
                    {

                    },
                    l.Tab("Supported Platforms", "Bool") with
                    {

                    }
                )
            }); ;
        }
    }
}
