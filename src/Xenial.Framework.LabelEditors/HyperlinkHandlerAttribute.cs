using System;
using System.ComponentModel;

using Xenial.Framework.Binding;

namespace DevExpress.Persistent.Base;

/// <summary>
/// 
/// </summary>
/// <param name="hyperlink"></param>
public delegate void HandleHyperlink(string hyperlink);

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class HyperlinkHandlerAttribute : Attribute, IBindableFunctorProvider<HandleHyperlink>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerAttribute"/> class. This
    /// assumes a public static method called HandleHyperlink that is compatible with the
    /// <see cref="HandleHyperlink" /> delegate.
    /// </summary>
    ///
    /// <example>
    /// <code>
    /// <![CDATA[
    /// [DomainComponent]
    /// public sealed class MyBusinessObject
    /// {
    ///     [LabelHyperlinkStringEditor]
    ///     [HyperlinkHandler]
    ///     //[HyperlinkHandler(nameof(HandleHyperlink))]
    ///     public string HyperLink { get; set; } = "<href="https://blog.xenial.io">Xenial Blog</href>"
    /// 
    ///     public static void HandleHyperlink(string hyperLink)
    ///     {
    ///         //Occures when the user clicks the link
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>

    public HyperlinkHandlerAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerAttribute"/> class.
    /// </summary>
    ///
    /// <param name="handleHyperlinkType">    Type of the handler. </param>

    public HyperlinkHandlerAttribute(Type handleHyperlinkType)
        => HandleHyperlinkType = handleHyperlinkType ?? throw new ArgumentNullException(nameof(handleHyperlinkType));

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerAttribute" /> class.
    /// </summary>
    ///
    /// <param name="handleHyperlinkType">                  Type of the hanlder. </param>
    /// <param name="handleHyperlinkMethodName">    Name of the hanlder method. </param>
    public HyperlinkHandlerAttribute(Type handleHyperlinkType, string handleHyperlinkMethodName)
        => (HandleHyperlinkType, HandleHyperlinkMethodName)
        = (
            handleHyperlinkType ?? throw new ArgumentNullException(nameof(handleHyperlinkType)),
            handleHyperlinkMethodName ?? throw new ArgumentNullException(nameof(handleHyperlinkMethodName))
        );

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerAttribute" /> class.
    /// </summary>
    ///
    /// <param name="handleHyperlinkDelegate">  The handler delegate. </param>

    [EditorBrowsable(EditorBrowsableState.Never)]
    public HyperlinkHandlerAttribute(HandleHyperlink handleHyperlinkDelegate)
        => HandleHyperlinkDelegate = handleHyperlinkDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperlinkHandlerAttribute"/> class.
    /// </summary>
    ///
    /// <param name="handleHyperlinkMethodName">    Name of the handler method. </param>

    public HyperlinkHandlerAttribute(string handleHyperlinkMethodName)
        => HandleHyperlinkMethodName = handleHyperlinkMethodName;

    /// <summary>   Gets the type of the handler. </summary>
    ///
    /// <value> The type of the handler. </value>

    public Type? HandleHyperlinkType { get; internal set; }

    /// <summary>   Gets the build layout delegate. </summary>
    ///
    /// <value> The build layout delegate. </value>

    public HandleHyperlink? HandleHyperlinkDelegate { get; internal set; }

    /// <summary>   Gets the name of the handler method. </summary>
    ///
    /// <value> The name of the handler method. </value>

    public string? HandleHyperlinkMethodName { get; internal set; }

    string IBindableFunctorProvider<HandleHyperlink>.ConventionMethodName
        => "HandleHyperlink";

    string? IBindableFunctorProvider<HandleHyperlink>.MethodName
        => HandleHyperlinkMethodName;

    Type? IBindableFunctorProvider<HandleHyperlink>.DelegatedType
        => HandleHyperlinkType;

    HandleHyperlink? IBindableFunctorProvider<HandleHyperlink>.Delegate
        => HandleHyperlinkDelegate;
}

