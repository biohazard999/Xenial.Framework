//HintName: XenialActionAttribute.g.cs
using System;

using Xenial.ExpressApp;
using Xenial.ExpressApp.Actions;
using Xenial.ExpressApp.Templates;
using Xenial.Persistent.Base;

namespace Xenial
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class XenialActionAttribute : Attribute
    {
        public XenialActionAttribute() { }
        public string Caption { get; set; }
        public string ImageName { get; set; }
        public string Category { get; set; }
        public string DiagnosticInfo { get; set; }
        public string Id { get; set; }
        public string TargetViewId { get; set; }
        public string TargetObjectsCriteria { get; set; }
        public string ConfirmationMessage { get; set; }
        public string ToolTip { get; set; }
        public string Shortcut { get; set; }
        public Type TargetObjectType { get; set; }
        public Type TypeOfView { get; set; }
        public bool QuickAccess { get; set; }
        public object Tag { get; set; }
        public XenialPredefinedCategory PredefinedCategory { get; set; }
        public XenialSelectionDependencyType SelectionDependencyType { get; set; }
        public XenialActionMeaning ActionMeaning { get; set; }
        public XenialViewType TargetViewType { get; set; }
        public XenialNesting TargetViewNesting { get; set; }
        public XenialTargetObjectsCriteriaMode TargetObjectsCriteriaMode { get; set; }
        public XenialActionItemPaintStyle PaintStyle { get; set; }
    }
    
    public interface IDetailViewAction<T> { }
    
    public interface IListViewAction<T> { }
}
