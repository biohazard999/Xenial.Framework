using System;

namespace Xenial.Framework.Tests.ModelBuilders
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class EmptyCtorLessAttribute : Attribute
    {
        public string AttributeProperty { get; set; } = string.Empty;
    }
}
