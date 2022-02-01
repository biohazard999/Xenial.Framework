//HintName: XenialViewIdsAttribute.g.cs
using System;
using System.ComponentModel;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class XenialViewIdsAttribute : Attribute
    {
        public XenialViewIdsAttribute() { }
    }
}
