using DevExpress.ExpressApp.DC;

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
                    l.Tab("Demo", "") with
                    {
                        Children = new()
                        {
                            l.PropertyEditor(m => m.NormalSteps)
                        }
                    },
                    l.Tab("Installation", "") with
                    {

                    }
                )
            });
        }
    }
}
