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
        // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
        // apparently anything over 4K converts to nvarchar(max) on SqlServer
        public const int TextSizeIndexable = 1000;

        //https://docs.microsoft.com/de-de/sql/sql-server/maximum-capacity-specifications-for-sql-server?view=sql-server-ver15#ssde-objects
        public const int ByteSizeIndexable = 1700; //This is the limit for SqlServers maximum byte count per non clustered indexed 

        public Mail(Session session) : base(session) { }

        [RuleRequiredField(DefaultContexts.Save)]
        public MailAccount? Account { get; set; }

        public MailDirection Direction { get; set; } = MailDirection.Inbound;

        [Persistent("MessageDateTime")]
        [Indexed]
        public DateTime? MessageDateTime { get; set; }

        [Persistent("ReceivedDateTime")]
        [Indexed]
        public DateTime? ReceivedDateTime { get; set; }

        [Persistent("UUId")]
        [Indexed(Unique = true)]
        [RuleRequiredField(DefaultContexts.Save)]
        [RuleUniqueValue(DefaultContexts.Save)]
        public Guid UUId { get; set; }

        [Persistent("From")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? From { get; set; }

        [Persistent("FromAll")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? FromAll { get; set; }

        [Persistent("To")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? To { get; set; }

        [Persistent("ToAll")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? ToAll { get; set; }

        [Persistent("CC")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? CC { get; set; }

        [Persistent("BCC")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? BCC { get; set; }

        [Persistent("Subject")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? Subject { get; set; }

        [Persistent("Sent")]
        public DateTime? Sent { get; set; }

        [Persistent("TextBody")]
        [Size(SizeAttribute.Unlimited)]
        public string? TextBody { get; set; }

        [Persistent("HtmlBody")]
        [Size(SizeAttribute.Unlimited)]
        public string? HtmlBody { get; set; }

        [Persistent("MessageId")]
        [Size(255)]
        [Indexed]
        public string MessageId { get; set; }

        [Persistent("MessageIdHash")]
        [Size(255)]
        [Indexed]
        public string MessageIdHash { get; set; }

        [Persistent("FileName")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? FileName { get; set; }

        [Persistent("ImapFolderName")]
        [Size(TextSizeIndexable)]
        [Indexed]
        public string? ImapFolderName { get; set; }

        [Persistent("AttachmentCount")]
        [Indexed]
        public int? AttachmentCount { get; set; }

        [Persistent("MessageImportance")]
        public MailImportance MessageImportance { get; set; }

        [Persistent("MessagePriority")]
        public MailPriority MessagePriority { get; set; }

        [Persistent("MessagePriorityX")]
        public MailPriorityX MessagePriorityX { get; set; }

        [Persistent("Size")]
        public long? Size { get; set; }
    }

    public enum MailDirection
    {
        Outbound = 1,
        Inbound = 2,
    }

    public enum MailImportance
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    public enum MailPriority
    {
        NonUrgent = 0,
        Normal = 1,
        Urgent = 2
    }

    public enum MailPriorityX
    {
        Highest = 1,
        High = 2,
        Normal = 3,
        Low = 4,
        Lowest = 5
    }
}
