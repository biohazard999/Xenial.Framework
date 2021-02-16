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
        [Persistent("Name")]
        public string Name { get; set; }

        [RuleRequiredField(DefaultContexts.Save)]
        public AccountType AccountType { get; set; }

        [Persistent("IgnoreInvalidCertificate")]
        public bool IgnoreInvalidCertificate { get; set; }

        [Persistent("SecuritySocketOptions")]
        public MailAccountSecuritySocketOptions? SecuritySocketOptions { get; set; }

        [Persistent("ReceiveHost")]
        public string ReceiveHost { get; set; }

        [Persistent("ReceivePort")]
        public int? ReceivePort { get; set; }

        [Persistent("ReceiveUserName")]
        public string ReceiveUserName { get; set; }

        [Persistent("ReceivePassword")]
        public string ReceivePassword { get; set; }
    }

    public enum AccountType
    {
        Imap = 1,
        Pop3 = 2
    }

    //
    // Summary:
    //     Secure socket options.
    //
    // Remarks:
    //     Provides a way of specifying the SSL and/or TLS encryption that should be used
    //     for a connection.
    public enum MailAccountSecuritySocketOptions
    {
        //
        // Summary:
        //     No SSL or TLS encryption should be used.
        None = 0,
        //
        // Summary:
        //     Allow the MailKit.IMailService to decide which SSL or TLS options to use (default).
        //     If the server does not support SSL or TLS, then the connection will continue
        //     without any encryption.
        Auto = 1,
        //
        // Summary:
        //     The connection should use SSL or TLS encryption immediately.
        SslOnConnect = 2,
        //
        // Summary:
        //     Elevates the connection to use TLS encryption immediately after reading the greeting
        //     and capabilities of the server. If the server does not support the STARTTLS extension,
        //     then the connection will fail and a System.NotSupportedException will be thrown.
        StartTls = 3,
        //
        // Summary:
        //     Elevates the connection to use TLS encryption immediately after reading the greeting
        //     and capabilities of the server, but only if the server supports the STARTTLS
        //     extension.
        StartTlsWhenAvailable = 4
    }
}
