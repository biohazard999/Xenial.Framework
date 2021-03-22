using System;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects
{
    public class DemoTask : BaseObject
    {
        public DemoTask(Session session) : base(session) { }

        [Association("Contact-DemoTask")]
        public XPCollection<Contact> Contacts 
            => GetCollection<Contact>(nameof(Contacts));
    }
}