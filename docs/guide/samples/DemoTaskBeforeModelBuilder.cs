using System;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects 
{
    [DefaultClassOptions]
    [ModelDefault("Caption", "Task")]
    public class DemoTask : BaseObject 
    {
        public DemoTask(Session session) : base(session) { }

        [ToolTip("View, assign or remove contacts for the current task")]
        [Association("Contact-DemoTask")]
        public XPCollection<Contact> Contacts
        {
            get
            {
                return GetCollection<Contact>(nameof(Contacts));
            }
        }
    }
}