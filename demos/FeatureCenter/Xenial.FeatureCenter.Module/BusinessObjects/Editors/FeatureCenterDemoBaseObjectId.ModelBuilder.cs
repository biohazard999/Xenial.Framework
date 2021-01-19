using System;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public abstract class FeatureCenterDemoBaseObjectIdModelBuilder<TClassType> : ModelBuilder<TClassType>
        where TClassType : FeatureCenterEditorsBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCenterDemoBaseObjectIdModelBuilder`1"/> class.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        public FeatureCenterDemoBaseObjectIdModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();
            this.GenerateNoListView();
            this.GenerateNoLookupListView();
        }

        protected LayoutTabbedGroupItem BuildDemoLayout(LayoutBuilder<TClassType> l, Func<LayoutBuilder<TClassType>, LayoutGroupItem, LayoutGroupItem> demoPage)
            => l.TabbedGroup
            (
                l.Tab("Demo", "Weather_Lightning") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.NotSupportedHtml) with { ShowCaption = false, CaptionLocation = Locations.Top },
                        demoPage(l, l.LayoutGroup() with { Id = "DEMO_LAYOUT_GROUP" })
                    }
                }
                ,
                l.Tab("Installation", "ShipmentReceived") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.Installation) with { ShowCaption = false, CaptionLocation = Locations.Top },
                    }
                },
                l.Tab("Usage", "Actions_Settings") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.Usage) with { ShowCaption = false, CaptionLocation = Locations.Top },
                    }
                },
                l.Tab("Remarks", "Actions_Info") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.Remarks) with { ShowCaption = false, CaptionLocation = Locations.Top },
                    }
                },
                l.Tab("Demo-Code", "ToggleFieldCodes") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.DemoCode) with { ShowCaption = false, CaptionLocation = Locations.Top },
                    }
                },
                l.Tab("Supported Platforms", "Bool") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.SupportedPlatforms) with { ShowCaption = false, CaptionLocation = Locations.Top },
                    }
                },
                l.Tab("Documentation", "DocumentStatistics") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.Documentation) with { ShowCaption = false, CaptionLocation = Locations.Top },
                    }
                }
        );
    }
}
