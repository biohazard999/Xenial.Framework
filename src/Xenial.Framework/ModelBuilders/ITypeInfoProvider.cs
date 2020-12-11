using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITypeInfoProvider
    {
        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <value>
        /// The type information.
        /// </value>
        ITypeInfo TypeInfo { get; }
    }
}
