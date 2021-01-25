using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace MailClient.Module.BusinessObjects
{
    [Persistent("Mails")]
    public class Mail : MailBaseObjectId
    {
        public Mail(Session session) : base(session) { }

        [RuleRequiredField(DefaultContexts.Save)]
        public MailAccount Account { get; set; }
        public MailDirection Direction { get; set; }

        [Persistent("From")]
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        [Size(50000)]
        [Indexed]
        public string From { get; set; }

        [Persistent("To")]
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        [Size(50000)]
        [Indexed]
        public string To { get; set; }

        [Persistent("CC")]
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        [Size(50000)]
        [Indexed]
        public string CC { get; set; }

        [Persistent("BCC")]
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        [Size(50000)]
        [Indexed]
        public string BCC { get; set; }

        [Persistent("Subject")]
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        [Size(50000)]
        [Indexed]
        public string Subject { get; set; }

        [Persistent("Sent")]
        public DateTime? Sent { get; set; }

        [Persistent("TextBody")]
        [Size(SizeAttribute.Unlimited)]
        public string TextBody { get; set; }

        [Persistent("HtmlBody")]
        [Size(SizeAttribute.Unlimited)]
        public string HtmlBody { get; set; }
    }

    public enum MailDirection
    {
        Outbound = 1,
        Inbound = 2,
    }
}
