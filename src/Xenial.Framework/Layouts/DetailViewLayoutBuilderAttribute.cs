using System;
using System.ComponentModel;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts
{
    /// <summary>
    /// Class LayoutBuilderAttribute. Implements the <see cref="System.Attribute" />
    /// </summary>
    ///
    /// <example>
    /// <code>
    /// [DomainComponent]
    /// [DetailViewLayoutBuilder]
    /// public sealed class MyBusinessObject
    /// {
    ///     public static Layout BuildLayout()
    ///     {
    ///         return new Layout();
    ///     }
    /// }
    /// </code>
    /// </example>
    ///
    /// <seealso cref="Attribute"/>
    /// <seealso cref="System.Attribute"/>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [XenialCheckLicence]
    public sealed partial class DetailViewLayoutBuilderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailViewLayoutBuilderAttribute"/> class. This
        /// assumes a public static method called BuildLayout that is compatible with the
        /// <see cref="BuildLayoutFunctor" /> delegate.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// [DomainComponent]
        /// [DetailViewLayoutBuilder]
        /// public sealed class MyBusinessObject
        /// {
        ///     public static Layout BuildLayout()
        ///     {
        ///         return new Layout();
        ///     }
        /// }
        /// </code>
        /// </example>

        public DetailViewLayoutBuilderAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailViewLayoutBuilderAttribute"/> class.
        /// </summary>
        ///
        /// <param name="generatorType">    Type of the layout builder. </param>

        public DetailViewLayoutBuilderAttribute(Type generatorType)
            => GeneratorType = generatorType ?? throw new ArgumentNullException(nameof(generatorType));

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailViewLayoutBuilderAttribute" /> class.
        /// </summary>
        ///
        /// <param name="generatorType">            Type of the generator. </param>
        /// <param name="buildLayoutMethodName">    Name of the build layout method. </param>

        public DetailViewLayoutBuilderAttribute(Type generatorType, string buildLayoutMethodName)
            => (GeneratorType, BuildLayoutMethodName)
            = (
                generatorType ?? throw new ArgumentNullException(nameof(generatorType)),
                buildLayoutMethodName ?? throw new ArgumentNullException(nameof(buildLayoutMethodName))
            );

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailViewLayoutBuilderAttribute" /> class.
        /// </summary>
        ///
        /// <param name="buildLayoutDelegate">  The build layout delegate. </param>

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DetailViewLayoutBuilderAttribute(BuildLayoutFunctor buildLayoutDelegate)
            => BuildLayoutDelegate = buildLayoutDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailViewLayoutBuilderAttribute"/> class.
        /// </summary>
        ///
        /// <param name="buildLayoutMethodName">    Name of the build layout method. </param>

        public DetailViewLayoutBuilderAttribute(string buildLayoutMethodName)
            => BuildLayoutMethodName = buildLayoutMethodName;

        /// <summary>   Gets the type of the layout builder. </summary>
        ///
        /// <value> The type of the layout builder. </value>

        public Type? GeneratorType { get; internal set; }

        /// <summary>   Gets the build layout delegate. </summary>
        ///
        /// <value> The build layout delegate. </value>

        public BuildLayoutFunctor? BuildLayoutDelegate { get; internal set; }

        /// <summary>   Gets the name of the build layout method. </summary>
        ///
        /// <value> The name of the build layout method. </value>

        public string? BuildLayoutMethodName { get; internal set; }

        ///// <summary>
        ///// Gets or sets the detail view identifier.
        ///// </summary>
        ///// <value>The detail view identifier.</value>
        //public string DetailViewId { get; set; }
    }

    /// <summary>   Delegate BuildLayoutDelegate. </summary>
    ///
    /// <returns>   Layout. </returns>

    public delegate Layout BuildLayoutFunctor();
}
