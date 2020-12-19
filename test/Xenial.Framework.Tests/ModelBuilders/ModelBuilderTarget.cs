using System;
using System.Collections;

namespace Xenial.Framework.Tests.ModelBuilders
{
    public class ModelBuilderTarget
    {
        public IList ListProperty { get; } = Array.Empty<object>();
        public string StringProperty { get; } = string.Empty;
        public string? NullableStringProperty { get; }
        public bool BoolProperty { get; }
        public bool? NullableBoolProperty { get; }
    }
}
