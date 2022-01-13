//HintName: XenialLayoutBuilderAttribute.g.cs
using System;
using System.ComponentModel;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class XenialLayoutBuilderAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class XenialExpandMemberAttribute : Attribute
    {
        public string ExpandMember { get; private set; }
        
        internal XenialExpandMemberAttribute(string expandMember)
        {
            this.ExpandMember = expandMember;
        }
    }
}
