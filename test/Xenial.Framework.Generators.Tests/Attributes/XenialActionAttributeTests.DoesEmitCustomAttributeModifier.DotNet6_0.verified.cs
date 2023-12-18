//HintName: XenialActionAttribute.g.cs
using System;
using System.Runtime.CompilerServices;

namespace Xenial
{
    [CompilerGenerated]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class XenialActionAttribute : Attribute
    {
        public XenialActionAttribute() { }
        public string Caption { get; set; }
        public string ImageName { get; set; }
        public string Category { get; set; }
        public string DiagnosticInfo { get; set; }
        public string TargetViewId { get; set; }
        public string[] TargetViewIds { get; set; }
        public string TargetObjectsCriteria { get; set; }
        public string ConfirmationMessage { get; set; }
        public string ToolTip { get; set; }
        public string Shortcut { get; set; }
        public string Id { get; set; }
        public Type TargetObjectType { get; set; }
        public Type TypeOfView { get; set; }
        public bool QuickAccess { get; set; }
        public object Tag { get; set; }
        public DevExpress.Persistent.Base.PredefinedCategory PredefinedCategory { get; set; }
        public DevExpress.ExpressApp.Actions.SelectionDependencyType SelectionDependencyType { get; set; }
        public DevExpress.ExpressApp.Actions.ActionMeaning ActionMeaning { get; set; }
        public DevExpress.ExpressApp.ViewType TargetViewType { get; set; }
        public DevExpress.ExpressApp.Nesting TargetViewNesting { get; set; }
        public DevExpress.ExpressApp.Actions.TargetObjectsCriteriaMode TargetObjectsCriteriaMode { get; set; }
        public DevExpress.ExpressApp.Templates.ActionItemPaintStyle PaintStyle { get; set; }
    }
}
