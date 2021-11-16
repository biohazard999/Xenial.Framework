//HintName: XenialImageNamesAttribute.g.cs
using System;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class XenialImageNamesAttribute : Attribute
    {
        internal XenialImageNamesAttribute() { }
        
        public bool Sizes { get; set; }
        public bool SmartComments { get; set; }
        public string DefaultImageSize { get; set; } = "16x16";
    }
}
