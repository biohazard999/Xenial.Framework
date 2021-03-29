using MailClient.Module.BusinessObjects;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class ModelBuilderDomainExtension
    {
        public static IModelBuilder<TClassType> HasExportFormat<TClassType>(
            this IModelBuilder<TClassType> modelBuilder, 
            string exportFormat = null
        )
        {
            if (exportFormat == null)
            {
                return modelBuilder.WithAttribute<ExportFormatAttribute>();
            }

            return modelBuilder.WithAttribute(new ExportFormatAttribute(exportFormat));
        }

        public static IPropertyBuilder<TPropertyType, TClassType> HasExportFormat<TPropertyType, TClassType>(
            this IPropertyBuilder<TPropertyType, TClassType> propertyBuilder, 
            string exportFormat = null
        )
        {
            if (exportFormat == null)
            {
                return propertyBuilder.WithAttribute<ExportFormatAttribute>();
            }

            return propertyBuilder.WithAttribute(new ExportFormatAttribute(exportFormat));
        }
    }
}
