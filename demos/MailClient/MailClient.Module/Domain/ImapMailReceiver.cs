using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using MailClient.Module.BusinessObjects;

using MailKit.Net.Imap;
using MailKit.Security;

namespace MailClient.Module.Domain
{
    public class ImapMailReceiver
    {
        readonly Func<Type, IObjectSpace> objectSpaceFactory;

        public ImapMailReceiver(Func<Type, IObjectSpace> objectSpaceFactory)
            => this.objectSpaceFactory = objectSpaceFactory;

        public async IAsyncEnumerable<Mail> ReceiveAsync(int mailConfigId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var os = objectSpaceFactory(typeof(Mail));

            static async Task CommitChangesAsync(IObjectSpace objectSpace)
            {
                if (objectSpace is IObjectSpaceAsync osAsync)
                {
                    await osAsync.CommitChangesAsync();
                }
                else
                {
                    objectSpace.CommitChanges();
                }
            }

            static async Task<MailAccount?> FindMailAccountAsync(IObjectSpace objectSpace, int mailConfigId)
            {
                if (objectSpace is IObjectSpaceAsync osAsync)
                {
                    return (MailAccount?)await osAsync.GetObjectByKeyAsync(typeof(MailAccount), mailConfigId);
                }

                return (MailAccount?)objectSpace.GetObjectByKey(typeof(MailAccount), mailConfigId);
            }

            static SecureSocketOptions GetSecuritySocketOptions(MailAccountSecuritySocketOptions? options)
            {
                return options switch
                {
                    MailAccountSecuritySocketOptions.None => SecureSocketOptions.None,
                    MailAccountSecuritySocketOptions.SslOnConnect => SecureSocketOptions.SslOnConnect,
                    MailAccountSecuritySocketOptions.StartTls => SecureSocketOptions.StartTls,
                    MailAccountSecuritySocketOptions.StartTlsWhenAvailable => SecureSocketOptions.StartTlsWhenAvailable,
                    _ => SecureSocketOptions.Auto
                };
            }

            var mailConfig = await FindMailAccountAsync(os, mailConfigId);
            if (mailConfig is null)
            {
                yield break;
            }

            using var imapClient = new ImapClient();

            if (mailConfig.IgnoreInvalidCertificate)
            {
                imapClient.ServerCertificateValidationCallback
                    = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            await imapClient.ConnectAsync(
                mailConfig.ReceiveHost,
                mailConfig.ReceivePort ?? 993,
                options: GetSecuritySocketOptions(mailConfig.SecuritySocketOptions),
                cancellationToken: cancellationToken
            );

            imapClient.AuthenticationMechanisms.Remove("XOAUTH2");

            await imapClient.AuthenticateAsync(
                mailConfig.ReceiveUserName,
                mailConfig.ReceivePassword,
                cancellationToken: cancellationToken
            );

            yield break;
        }
    }
}
