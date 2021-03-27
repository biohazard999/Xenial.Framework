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

            //For demo simplicity
            //this could come from configuration, or a database
            if(new Random().Next() % 2 == 0)
            {
                var caption = "contacts";
                For(m => m.Contacts)
                    .HasTooltip($"View, assign or remove {caption} for the current task");
            }
            else
            {
                var caption = "assigned contacts";
                For(m => m.Contacts)
                    .HasTooltip($"View, assign or remove {caption} for the current task");
            }
        }
    }
}
