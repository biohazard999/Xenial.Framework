using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;

using Xenial;

namespace Acme.Module.Commands;

internal class DomainComponent
{
}

//[XenialAction(Caption = "Simple", ImageName = ImageNames.aac)]
//internal partial class SimpleCommand : IDetailViewAction<DomainComponent>
//{
//    internal partial void Execute(DomainComponent domainComponent)
//    {

//    }
//}

//[XenialAction(Caption = "Injected Record", ImageName = ImageNames.aac)]
//internal partial class InjectedIntoExecuteCommand : IDetailViewAction<DomainComponent>
//{
//    internal partial void Execute(
//        DomainComponent domainComponent,
//        IObjectSpace os,
//        XafApplication xafApplication
//    )
//    {

//    }
//}

[XenialAction(Caption = "Injected Record", ImageName = ImageNames.aac)]
internal partial record InjectedIntoRecordCommand(IObjectSpace ObjectSpace, XafApplication Application) : IDetailViewAction<DomainComponent>
{
    internal partial void Execute(DomainComponent domainComponent)
    {

    }
}
