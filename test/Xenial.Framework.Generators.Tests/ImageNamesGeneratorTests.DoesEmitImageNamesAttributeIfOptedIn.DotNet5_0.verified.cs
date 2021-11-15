//HintName: XenialImageNamesAttribute.g.cs
using System;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class XenialImageNamesAttribute : Attribute
    {
        internal XenialImageNamesAttribute() { }
        
        internal bool Sizes { get; set; }
        internal bool SmartComments { get; set; }
    }
}
