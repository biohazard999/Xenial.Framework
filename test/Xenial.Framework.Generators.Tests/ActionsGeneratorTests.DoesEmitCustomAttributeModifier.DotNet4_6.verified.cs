//HintName: XenialActionAttribute.g.cs
using System;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class XenialActionAttribute : Attribute
    {
        public XenialActionAttribute() { }
        public string Caption { get; set; }
        public string ImageName { get; set; }
        public string Category { get; set; }
    }
    
    public interface IDetailViewAction<T> { }
    
    public interface IListViewAction<T> { }
}
