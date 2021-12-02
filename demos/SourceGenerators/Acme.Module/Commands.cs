using System;

using DevExpress.ExpressApp;

using Xenial;

namespace Acme.Module.Commands;

public class DomainComponent
{
}

[XenialAction(Caption = "Fooooooo", ImageName = ImageNames.aac, Category = "View")]
public partial record JörgsAction(XafApplication Application, IObjectSpace ObjectSpace) : IDetailViewAction<DomainComponent>
{
    public partial Task Execute(DomainComponent targetObject)
        => throw new NotImplementedException();
}


//[XenialAction(Caption = "Fooooooo"
//    , ImageName = ImageNames.aac,
//    Category = "View"
//    )]
//public partial record JörgsAction1(XafApplication Application, IObjectSpace ObjectSpace) : IDetailViewAction<DomainComponent>
//{
//    public partial Task Execute(DomainComponent targetObject)
//        => throw new NotImplementedException();
//}

//internal class Person
//{
//}
//[XenialAction(Caption = "Simple", ImageName = ImageNames.aac)]
//internal partial class SimpleCommand : IDetailViewAction<DomainComponent>
//{
//    //internal partial void Execute(DomainComponent domainComponent)
//    //{

//    //}

//    public partial Task Execute(DomainComponent targetObject, XafApplication application, IObjectSpace objectSpace) => throw new NotImplementedException();
//}

//[XenialAction(Caption = "Injected Record", ImageName = ImageNames.aac)]
//internal partial class InjectedIntoExecuteCommand : IDetailViewAction<Person>
//{
//    internal partial void Execute(
//        Person erson
//    )
//    {

//    }
//}


////[XenialAction(Caption = "Injected Record", ImageName = ImageNames.aac)]
////internal partial record InjectedIntoRecordCommand(
////    IObjectSpace ObjectSpace,
////    XafApplication Application
////) : IDetailViewAction<DomainComponent>
////{
////    internal async partial Task Execute(DomainComponent domainComponent)
////    {
////        var foo = new InjectedIntoRecordCommand1("fjsdlkfjslsfdjlks", 123);

////        foo = foo with { Dsjakljdsflw = 322313 };

////    }
////}

////internal partial record InjectedIntoRecordCommand1(string BAR, int Dsjakljdsflw)
////{
////}

//[XenialAction]
//internal partial record AktÖffnen(
//    XafApplication Application,
//    IObjectSpace ObjectSpace
//)
//    : IDetailViewAction<Person>
//{
//    partial void Execute(Person targetObject) => throw new NotImplementedException();
//}
