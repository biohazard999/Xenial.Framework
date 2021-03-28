using System;
using DevExpress.ExpressApp.DC;
using Xenial.Framework.ModelBuilder;

namespace MainDemo.Module.BusinessObjects
{
    public class SensorModelBuilder : ModelBuilder<Sensor>
    {
        public SensorModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            for (int i = 1; i <= 5; i++)
            {
                For($"Value{i}")
                    .HasCaption($"Sensor Value {i}");
            }
        }
    }
}