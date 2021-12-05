using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

using Xenial;
using Xenial.Persistent.Base;

namespace Acme.Module.Commands;

public class DomainComponent
{
}

[XenialAction(
    Caption = "Yo Alex",
    ImageName = ImageNames.aac,
    Category = "View",
    PredefinedCategory = XenialPredefinedCategory.View,
    ActionMeaning = Xenial.ExpressApp.Actions.XenialActionMeaning.Cancel,
    TargetObjectType = typeof(DomainComponent),
    QuickAccess = true,
    Tag = "???",
    SelectionDependencyType = Xenial.ExpressApp.Actions.XenialSelectionDependencyType.RequireMultipleObjects,
    ConfirmationMessage = "Confirm",
    DiagnosticInfo = "Diagnostic",
    Id = "FOOID",
    PaintStyle = Xenial.ExpressApp.Templates.XenialActionItemPaintStyle.CaptionAndImage,
    Shortcut = "CtrlC",
    TargetObjectsCriteria = "1=2",
    TargetObjectsCriteriaMode = Xenial.ExpressApp.Actions.XenialTargetObjectsCriteriaMode.TrueForAll,
    TargetViewId = "TargetViewIdFoo",
    TargetViewNesting = Xenial.ExpressApp.XenialNesting.Nested,
    TargetViewType = Xenial.ExpressApp.XenialViewType.DashboardView,
    ToolTip = "Tooltip to the action",
    TypeOfView = typeof(DashboardView)
//PredefinedCategory = XenialPredefinedCategory.Export
)]
public partial record AlexAction(XafApplication Application, IObjectSpace ObjectSpace)
    : IDetailViewAction<DomainComponent>
{
    public partial void Execute(DomainComponent obj)
    {
        var sa = new SimpleAction()
        {
        };
        throw null;
    }
}
