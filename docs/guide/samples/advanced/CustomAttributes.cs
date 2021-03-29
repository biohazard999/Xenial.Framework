using System;
using DevExpress.ExpressApp.DC;
using Xenial.Framework.ModelBuilder;

namespace MainDemo.Module.BusinessObjects
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ExportFormatAttribute : Attribute
    {
        public string Format { get; }

        public ExportFormatAttribute(string format)
        {
            Format = format;
        }

        public ExportFormatAttribute()
        {
            Format = "My Default Format";
        }
    }

    public class DemoTaskModelBuilder : ModelBuilder<DemoTask>
    {
        public DemoTaskModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.WithAttribute(new ExportFormatAttribute("My Format"));

            For(m => m.Contacts)
                .WithAttribute<ExportFormatAttribute>();
        }
    }
}