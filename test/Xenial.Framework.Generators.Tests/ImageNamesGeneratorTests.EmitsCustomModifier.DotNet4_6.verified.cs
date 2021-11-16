//HintName: XenialImageNamesAttribute.g.cs
using System;
using System.ComponentModel;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class XenialImageNamesAttribute : Attribute
    {
        public XenialImageNamesAttribute() { }
        
        public bool Sizes { get; set; }
        public bool SmartComments { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DefaultImageSize { get; set; } = "16x16";
    }
}
