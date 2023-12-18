using System;
using System.Collections.Generic;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    public class Position : BaseObject
    {
        public Position(Session session)
            : base(session)
        {
        }
        private string title;
        [RuleRequiredField("RuleRequiredField for Position.Title", DefaultContexts.Save)]
        public string Title
        {
            get => title;
            set => SetPropertyValue(nameof(Title), ref title, value);
        }
        [Association("Departments-Positions")]
        public XPCollection<Department> Departments => GetCollection<Department>(nameof(Departments));
    }
}
