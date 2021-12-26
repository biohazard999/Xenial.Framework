using System;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

namespace MainDemo.Module.BusinessObjects
{
    public class DemoTaskModelBuilder : ModelBuilder<DemoTask>
    {
        public DemoTaskModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("Task")
                .WithDefaultClassOptions();

            For(m => m.Employees)
                .HasTooltip("View, assign or remove employees for the current task");
        }
    }
}
