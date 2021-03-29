namespace Xenial.Framework.ModelBuilders
{
    public static partial class ModelBuilderDomainExtension
    {
        public static IPropertyBuilder<TPropertyType, TClassType> AsNumeric<TPropertyType, TClassType>(
            this IPropertyBuilder<TPropertyType, TClassType> propertyBuilder
        )
        {
            return propertyBuilder
                .HasExportFormat("n0")
                .HasDisplayFormat("{0:n0}")
                .HasEditMask("n0");
        }
    }
}
