using System;
using System.Reflection;

#pragma warning disable CA1032 //By design
#pragma warning disable CA2237 //By design

namespace Xenial.Framework.Utils
{
    /// <summary>
    /// Exception for signalling resource not found errors. This class cannot be inherited.
    /// </summary>
    ///
    /// <seealso cref="Exception"/>
    /// <seealso cref="System.Exception"/>

    public sealed class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="assembly">     The assembly. </param>
        /// <param name="resourceName"> Name of the resource. </param>

        public ResourceNotFoundException(Assembly assembly, string? resourceName) : base($"Resource '{resourceName}' was not found in Assembly '{assembly?.GetName()?.Name}'")
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            ResourceName = resourceName;
        }

        /// <summary>   Gets the assembly. </summary>
        ///
        /// <value> The assembly. </value>

        public Assembly Assembly { get; }

        /// <summary>   Gets the name of the resource. </summary>
        ///
        /// <value> The name of the resource. </value>

        public string? ResourceName { get; }

        /// <summary>   Gets the resource path. </summary>
        ///
        /// <value> The resource path. </value>

        public string ResourcePath => $"{Assembly.GetName().Name}.{ResourceName}";
    }
}
