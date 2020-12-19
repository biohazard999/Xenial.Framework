using System;
using System.Collections;

namespace Xenial.Framework.Tests.ModelBuilders
{
    public class ModelBuilderTarget
    {
        public IList ListProperty { get; } = Array.Empty<object>();
        public string StringProperty { get; } = string.Empty;
    }
}
