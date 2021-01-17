using System;
using System.Collections.Generic;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public abstract class FeatureCenterDemoBaseObjectIdModelBuilder<TClassType> : ModelBuilder<TClassType>
        where TClassType : FeatureCenterDemoBaseObjectId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCenterDemoBaseObjectIdModelBuilder`1"/> class.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        public FeatureCenterDemoBaseObjectIdModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        protected LayoutTabbedGroupItem BuildDemoLayout(LayoutBuilder<TClassType> l, Func<LayoutBuilder<TClassType>, IEnumerable<LayoutItemNode>> demoPage)
            => l.TabbedGroup
            (
                l.Tab("Demo", "Weather_Lightning") with
                {
                    Children = new()
                    {
                        demoPage(l)
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
                    Children = new()
                    {
                        l.PropertyEditor(m => m.Remarks),
                    }
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
                    Children = new()
                    {
                        l.PropertyEditor(m => m.SupportedPlatforms),
                    }
                },
                l.Tab("Documentation", "DocumentStatistics") with
                {
                    Children = new()
                    {
                        l.PropertyEditor(m => m.Documentation),
                    }
                }
        );
    }
}
