using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MailClient.Module.BusinessObjects
{
    [Persistent("MailSettings")]
    public class MailSettings : MailBaseObjectId
    {
        public MailSettings(Session session) : base(session) { }

        [RuleRequiredField(DefaultContexts.Save)]
        [RuleUniqueValue(DefaultContexts.Save)]
        [Indexed(Unique = true)]
        [Persistent("StoragePath")]
        public string StoragePath { get; set; }
    }
}
