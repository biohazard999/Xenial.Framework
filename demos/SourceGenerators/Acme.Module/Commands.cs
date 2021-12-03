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
    TargetObjectType = typeof(DomainComponent)
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
