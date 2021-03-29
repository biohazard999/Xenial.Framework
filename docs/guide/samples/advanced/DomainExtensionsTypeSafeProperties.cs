using System;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class ModelBuilderDomainExtension
    {
        private static const string LongDateGeneralFormat = "{0:G}";

        public static IPropertyBuilder<DateTime, TClassType> HasLongGeneralDateFormat<TClassType>(this IPropertyBuilder<DateTime, TClassType> propertyBuilder)
        {
            return propertyBuilder.HasDisplayFormat(LongDateGeneralFormat);
        }

        public static IPropertyBuilder<DateTime?, TClassType> HasLongGeneralDateFormat<TClassType>(this IPropertyBuilder<DateTime?, TClassType> propertyBuilder)
        {
            return propertyBuilder.HasDisplayFormat(LongDateGeneralFormat);
        }
    }
}
