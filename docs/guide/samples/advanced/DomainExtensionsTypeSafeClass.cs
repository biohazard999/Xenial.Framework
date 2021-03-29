using YourSolutionName.Module.BusinessObjects;

namespace Xenial.Framework.ModelBuilders
{
    public static partial class ModelBuilderDomainExtension
    {
        public static IModelBuilder<TClassType> AsAuditableLight<TClassType>(this IModelBuilder<TClassType> modelBuilder)
            where TClassType : class, IAuditableLight
        {
            const string displayFormat = "{0:G}";

            modelBuilder
                .For(m => m.CreatedBy)
                .NotAllowingEdit();

            modelBuilder
                .For(m => m.CreatedOn)
                .NotAllowingEdit()
                .HasDisplayFormat(displayFormat);

            modelBuilder
                .For(m => m.UpdatedBy)
                .NotAllowingEdit();

            modelBuilder
                .For(m => m.UpdatedOn)
                .NotAllowingEdit()
                .HasDisplayFormat(displayFormat);

            return modelBuilder;
        }
    }
}
