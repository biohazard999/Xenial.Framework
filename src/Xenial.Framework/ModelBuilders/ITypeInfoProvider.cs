using DevExpress.ExpressApp.DC;

namespace Xenial.Framework.ModelBuilders;

/// <summary>   Interface for type information provider. </summary>
public interface ITypeInfoProvider
{
    /// <summary>   Gets the type information. </summary>
    ///
    /// <value> The type information. </value>

    ITypeInfo TypeInfo { get; }
}
