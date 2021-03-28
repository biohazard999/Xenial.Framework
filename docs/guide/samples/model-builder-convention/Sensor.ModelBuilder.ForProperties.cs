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

            ForProperties(
                m => m.Value1,
                m => m.Value2,
                m => m.Value3,
                m => m.Value4,
                m => m.Value5
            ).HasDisplayFormat("{0:n}");
        }
    }
}