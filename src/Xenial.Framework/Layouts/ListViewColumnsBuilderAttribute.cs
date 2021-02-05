﻿using System;
using System.ComponentModel;

namespace Xenial.Framework.Layouts
{
    /// <summary>
    /// Class ListViewColumnsBuilderAttribute. This class cannot be inherited.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <autogeneratedoc />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [XenialCheckLicence]
    public sealed partial class ListViewColumnsBuilderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewColumnsBuilderAttribute"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public ListViewColumnsBuilderAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewColumnsBuilderAttribute"/> class.
        /// </summary>
        /// <param name="generatorType">Type of the column builder.</param>
        /// <autogeneratedoc />
        public ListViewColumnsBuilderAttribute(Type generatorType)
            => GeneratorType = generatorType ?? throw new ArgumentNullException(nameof(generatorType));

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewColumnsBuilderAttribute" /> class.
        /// </summary>
        /// <param name="generatorType">Type of the generator.</param>
        /// <param name="buildColumnsMethodName">Name of the build columns method.</param>
        /// <exception cref="ArgumentNullException">generatorType</exception>
        /// <exception cref="ArgumentNullException">buildLayoutMethodName</exception>
        public ListViewColumnsBuilderAttribute(Type generatorType, string buildColumnsMethodName)
            => (GeneratorType, BuildColumnsMethodName)
            = (
                generatorType ?? throw new ArgumentNullException(nameof(generatorType)),
                buildColumnsMethodName ?? throw new ArgumentNullException(nameof(buildColumnsMethodName))
            );

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewColumnsBuilderAttribute"/> class.
        /// </summary>
        /// <param name="buildColumnsDelegate">The build columns delegate.</param>
        /// <autogeneratedoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ListViewColumnsBuilderAttribute(BuildColumnsFunctor buildColumnsDelegate)
            => BuildColumnsDelegate = buildColumnsDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewColumnsBuilderAttribute"/> class.
        /// </summary>
        /// <param name="buildColumnsMethodName">Name of the build columns method.</param>
        /// <autogeneratedoc />
        public ListViewColumnsBuilderAttribute(string buildColumnsMethodName)
            => BuildColumnsMethodName = buildColumnsMethodName;

        /// <summary>
        /// Gets the type of the columns builder.
        /// </summary>
        /// <value>The type of the columns builder.</value>
        /// <autogeneratedoc />
        public Type? GeneratorType { get; internal set; }

        /// <summary>
        /// Gets the build columns delegate.
        /// </summary>
        /// <value>The build columns delegate.</value>
        /// <autogeneratedoc />
        public BuildColumnsFunctor? BuildColumnsDelegate { get; internal set; }

        /// <summary>
        /// Gets the name of the build layout method.
        /// </summary>
        /// <value>The name of the build layout method.</value>
        /// <autogeneratedoc />
        public string? BuildColumnsMethodName { get; internal set; }

        ///// <summary>
        ///// Gets or sets the list view identifier.
        ///// </summary>
        ///// <value>The list view identifier.</value>
        //public string ListViewId { get; set; }
    }

    /// <summary>
    /// Delegate BuildColumnsDelegate
    /// </summary>
    /// <returns>Layout.</returns>
    /// <autogeneratedoc />
    public delegate Columns BuildColumnsFunctor();
}
