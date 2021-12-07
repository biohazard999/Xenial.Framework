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
    ActionMeaning = Xenial.ExpressApp.Actions.XenialActionMeaning.Cancel,
    QuickAccess = true,
    Tag = "???",
    ConfirmationMessage = "Confirm",
    DiagnosticInfo = "Diagnostic",
    Id = "FOOID",
    PaintStyle = Xenial.ExpressApp.Templates.XenialActionItemPaintStyle.CaptionAndImage,
    Shortcut = "CtrlC",
    TargetObjectsCriteria = "1=2",
    TargetObjectsCriteriaMode = Xenial.ExpressApp.Actions.XenialTargetObjectsCriteriaMode.TrueForAll,
    TargetViewId = "TargetViewIdFoo",
    TargetViewNesting = Xenial.ExpressApp.XenialNesting.Nested,
    ToolTip = "Tooltip to the action",


    SelectionDependencyType = Xenial.ExpressApp.Actions.XenialSelectionDependencyType.RequireMultipleObjects,
    TargetObjectType = typeof(DomainComponent),
    TargetViewType = Xenial.ExpressApp.XenialViewType.DashboardView,
    TypeOfView = typeof(DashboardView)
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

public class Foo : ViewController
{
    public Foo()
    {
        TargetViewType
    }
}
