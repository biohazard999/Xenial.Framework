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

            this.WithDefaultClassOptions()
                .HasCaption("Task");
        }
    }
}