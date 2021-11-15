//HintName: XenialImageNamesAttribute.g.cs
using System;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class XenialImageNamesAttribute : Attribute
    {
        public XenialImageNamesAttribute() { }
        
        public bool Sizes { get; set; }
        public bool SmartComments { get; set; }
    }
}
