using System;

namespace Xenial.Framework.Tests.ModelBuilders
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class EmptyCtorLessAttribute : Attribute
    {
        /// <summary>   Gets or sets the attribute property. </summary>
        ///
        /// <value> The attribute property. </value>

        public string AttributeProperty { get; set; } = string.Empty;
    }
}
