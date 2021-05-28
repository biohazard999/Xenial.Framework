using System;
using System.Collections;

namespace Xenial.Framework.Tests.ModelBuilders
{
    /// <summary>   A model builder target. </summary>
    public class ModelBuilderTarget
    {
        /// <summary>   Gets the list property. </summary>
        ///
        /// <value> The list property. </value>

        public IList ListProperty { get; } = Array.Empty<object>();

        /// <summary>   Gets the string property. </summary>
        ///
        /// <value> The string property. </value>

        public string StringProperty { get; } = string.Empty;

        /// <summary>   Gets the nullable string property. </summary>
        ///
        /// <value> The nullable string property. </value>

        public string? NullableStringProperty { get; }

        /// <summary>   Gets a value indicating whether the property. </summary>
        ///
        /// <value> True if property, false if not. </value>

        public bool BoolProperty { get; }

        /// <summary>   Gets the nullable bool property. </summary>
        ///
        /// <value> The nullable bool property. </value>

        public bool? NullableBoolProperty { get; }
    }
}
