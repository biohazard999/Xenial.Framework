using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

using System;
using System.Collections.Generic;
using System.Text;

namespace MailClient.Module.BusinessObjects
{
    [Persistent("MailAccounts")]
    public class MailAccount : MailBaseObjectId
    {
        public MailAccount(Session session) : base(session) { }

        [Indexed(Unique = true)]
        [RuleUniqueValue(DefaultContexts.Save)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string Name { get; set; }

        [RuleRequiredField(DefaultContexts.Save)]
        public AccountType AccountType { get; set; }
    }

    public enum AccountType
    {
        Imap = 1,
        Pop3 = 2
    }
}
