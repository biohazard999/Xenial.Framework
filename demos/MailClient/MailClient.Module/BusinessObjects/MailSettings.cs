using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

using System;
using System.Collections.Generic;
using System.Text;

namespace MailClient.Module.BusinessObjects
{
    [Persistent("MailSettings")]
    public class MailSettings : MailBaseObjectId
    {
        public MailSettings(Session session) : base(session) { }

        [Indexed(Unique = true)]
        [RuleUniqueValue(DefaultContexts.Save)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string Name { get; set; }
    }
}
