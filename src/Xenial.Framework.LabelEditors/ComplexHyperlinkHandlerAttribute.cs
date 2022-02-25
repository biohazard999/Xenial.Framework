using System;
using System.ComponentModel;

using Xenial.Framework.Binding;

namespace DevExpress.Persistent.Base;

/// <summary>
/// 
/// </summary>
/// <param name="Hyperlink"></param>
public sealed record HyperlinkContext(string Hyperlink);

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
public delegate void HandleHyperlinkComplex(HyperlinkContext context);

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class HyperlinkHandlerComplexAttribute : Attribute, IBindableFunctorProvider<HandleHyperlinkComplex>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerComplexAttribute"/> class. This
    /// assumes a public static method called HandleHyperlink that is compatible with the
    /// <see cref="HandleHyperlinkComplex" /> delegate.
    /// </summary>
    ///
    /// <example>
    /// <code>
    /// <![CDATA[
    /// [DomainComponent]
    /// public sealed class MyBusinessObject
    /// {
    ///     [LabelHyperlinkStringEditor]
    ///     [HyperlinkHandlerComplex]
    ///     //[HyperlinkHandlerComplex(nameof(HandleHyperlink))]
    ///     public string HyperLink { get; set; } = "<href="https://blog.xenial.io">Xenial Blog</href>"
    /// 
    ///     public static void HandleHyperlink(HyperlinkContext context)
    ///     {
    ///         //Occures when the user clicks the link
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>

    public HyperlinkHandlerComplexAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerComplexAttribute"/> class.
    /// </summary>
    ///
    /// <param name="handlerType">    Type of the handler. </param>

    public HyperlinkHandlerComplexAttribute(Type handlerType!!)
        => HandleHyperlinkType = handlerType;

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerComplexAttribute" /> class.
    /// </summary>
    ///
    /// <param name="handlerType">                  Type of the hanlder. </param>
    /// <param name="handleHyperlinkMethodName">    Name of the hanlder method. </param>
    public HyperlinkHandlerComplexAttribute(Type handlerType!!, string handleHyperlinkMethodName!!)
        => (HandleHyperlinkType, HandleHyperlinkMethodName)
        = (handlerType, handleHyperlinkMethodName);

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerComplexAttribute" /> class.
    /// </summary>
    ///
    /// <param name="hyperlinkHandler">  The handler delegate. </param>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public HyperlinkHandlerComplexAttribute(HandleHyperlinkComplex hyperlinkHandler)
        => HandleHyperlinkDelegate = hyperlinkHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerComplexAttribute"/> class.
    /// </summary>
    ///
    /// <param name="handleHyperlinkMethodName">    Name of the handler method. </param>

    public HyperlinkHandlerComplexAttribute(string handleHyperlinkMethodName)
        => HandleHyperlinkMethodName = handleHyperlinkMethodName;

    /// <summary>   Gets the type of the handler. </summary>
    ///
    /// <value> The type of the handler. </value>

    public Type? HandleHyperlinkType { get; internal set; }

    /// <summary>   Gets the build layout delegate. </summary>
    ///
    /// <value> The build layout delegate. </value>

    public HandleHyperlinkComplex? HandleHyperlinkDelegate { get; internal set; }

    /// <summary>   Gets the name of the handler method. </summary>
    ///
    /// <value> The name of the handler method. </value>

    public string? HandleHyperlinkMethodName { get; internal set; }

    string IBindableFunctorProvider<HandleHyperlinkComplex>.ConventionMethodName
        => "HandleHyperlink";

    string? IBindableFunctorProvider<HandleHyperlinkComplex>.MethodName
        => HandleHyperlinkMethodName;

    Type? IBindableFunctorProvider<HandleHyperlinkComplex>.DelegatedType
        => HandleHyperlinkType;

    HandleHyperlinkComplex? IBindableFunctorProvider<HandleHyperlinkComplex>.Delegate
        => HandleHyperlinkDelegate;
}

