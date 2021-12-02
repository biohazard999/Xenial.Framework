//HintName: XenialActionAttribute.g.cs
using System;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class XenialActionAttribute : Attribute
    {
        internal XenialActionAttribute() { }
        public string Caption { get; set; }
        public string ImageName { get; set; }
        public string Category { get; set; }
    }
    
    internal interface IDetailViewAction<T> { }
    
    internal interface IListViewAction<T> { }
}
