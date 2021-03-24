using System;
using DevExpress.ExpressApp.DC;
using Xenial.Framework.ModelBuilder;

namespace MainDemo.Module.BusinessObjects
{
    public class DemoTaskModelBuilder : ModelBuilder<DemoTask>
    {
        public DemoTaskModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            For(m => m.Contacts)
                .HasTooltip("View, assign or remove contacts for the current task");
        }
    }
}