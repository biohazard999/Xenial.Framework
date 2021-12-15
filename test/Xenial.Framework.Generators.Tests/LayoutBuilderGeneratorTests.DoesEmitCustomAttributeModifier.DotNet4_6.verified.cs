//HintName: XenialLayoutBuilderAttribute.g.cs
using System;
using System.ComponentModel;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class XenialLayoutBuilderAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class XenialExpandMemberAttribute : Attribute
    {
        public string ExpandMember { get; private set; }
        
        public XenialExpandMemberAttribute(string expandMember)
        {
            this.ExpandMember = expandMember;
        }
    }
}
