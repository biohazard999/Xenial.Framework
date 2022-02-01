//HintName: XenialExpandMemberAttribute.g.cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [CompilerGenerated]
    public sealed class XenialExpandMemberAttribute : Attribute
    {
        public string ExpandMember { get; private set; }
        
        public XenialExpandMemberAttribute(string expandMember)
        {
            this.ExpandMember = expandMember;
        }
    }
}
