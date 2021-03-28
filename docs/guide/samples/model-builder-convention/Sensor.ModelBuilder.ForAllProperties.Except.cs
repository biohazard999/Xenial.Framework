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

            ForAllProperties()
                .Except(
                    m => m.Value2,
                    m => m.Value4
                )
                .WithModelDefault(ModelDefaults.AllowEdit, false);
        }
    }
}