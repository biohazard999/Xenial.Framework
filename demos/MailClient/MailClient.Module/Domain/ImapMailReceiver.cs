using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.ExpressApp;

using MailClient.Module.BusinessObjects;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

using MimeKit;
using System.IO;

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

            var mailAccount = await FindMailAccountAsync(os, mailConfigId);
            if (mailAccount is null)
            {
                yield break;
            }

            using var imapClient = new ImapClient();

            if (mailAccount.IgnoreInvalidCertificate)
            {
                imapClient.ServerCertificateValidationCallback
                    = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            await imapClient.ConnectAsync(
                mailAccount.ReceiveHost,
                mailAccount.ReceivePort ?? 993,
                options: GetSecuritySocketOptions(mailAccount.SecuritySocketOptions),
                cancellationToken: cancellationToken
            );

            imapClient.AuthenticationMechanisms.Remove("XOAUTH2");

            await imapClient.AuthenticateAsync(
                mailAccount.ReceiveUserName,
                mailAccount.ReceivePassword,
                cancellationToken: cancellationToken
            );

            var inbox = imapClient.Inbox;

            await foreach (var mail in ReceiveAsync(mailAccount.Id, inbox, cancellationToken))
            {
                yield return mail;
            }

            yield break;
        }

        private async IAsyncEnumerable<Mail> ReceiveAsync(int mailAccountId, IMailFolder folder, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await folder.OpenAsync(FolderAccess.ReadOnly, cancellationToken: cancellationToken);

            var uniqueIds = await folder.FetchAsync(0, -1, MessageSummaryItems.UniqueId, cancellationToken: cancellationToken);

            var inboxCount = uniqueIds.Count;
            for (var current = inboxCount - 1; current > -1; current--)
            {
                using var headerStream = await folder.GetStreamAsync(uniqueIds[current].UniqueId, "HEADER", cancellationToken: cancellationToken);

                var headerList = HeaderList.Load(headerStream, cancellationToken: cancellationToken);

                var messageId = headerList.GetMessageId();
                var messageIdHash = headerList.GetMessageIdHash();

                var query = new MailQuery(objectSpaceFactory);

                var existingMails = query.GetExistingInboundMails(mailAccountId, messageId, messageIdHash);

                if (!existingMails.Any())
                {
                    using var os = objectSpaceFactory(typeof(Mail));
                    var uniqueId = uniqueIds[current].UniqueId;
                    using var rawMessageStream = await folder.GetStreamAsync(uniqueId, string.Empty, cancellationToken: cancellationToken);

                    var currentDateTime = DateTime.UtcNow;

                    var fileName = CreateFileName(mailAccountId, MailDirection.Inbound, currentDateTime);

                    using (var fileStream = File.Create(fileName))
                    {
                        rawMessageStream.Seek(0, SeekOrigin.Begin);
                        await rawMessageStream.CopyToAsync(fileStream);
                    }

                    rawMessageStream.Seek(0, SeekOrigin.Begin);

                    var message = MimeMessage.Load(rawMessageStream, cancellationToken: cancellationToken);

                }

                yield return (Mail)null;
            }
        }
    }

    internal record MailInfo(int Id, int MailAccountId, string? FileName, string MessageId, string MessageIdHash);

    internal class MailQuery
    {
        private readonly Func<Type, IObjectSpace> objectSpaceFactory;
        public MailQuery(Func<Type, IObjectSpace> objectSpaceFactory)
            => this.objectSpaceFactory = objectSpaceFactory ?? throw new ArgumentNullException(nameof(objectSpaceFactory));

        public IList<MailInfo> GetExistingInboundMails(
            int mailAccountId,
            string messageId,
            string messageIdHash
        ) => GetExistingMails(
            mailAccountId,
            messageId,
            messageIdHash,
            MailDirection.Inbound
        );
        public IList<MailInfo> GetExistingOutboundMails(
            int mailAccountId,
            string messageId,
            string messageIdHash
        ) => GetExistingMails(
            mailAccountId,
            messageId,
            messageIdHash,
            MailDirection.Outbound
        );

        public IList<MailInfo> GetExistingMails(
            int mailAccountId,
            string messageId,
            string messageIdHash,
            MailDirection direction
        )
        {
            using var os = objectSpaceFactory(typeof(Mail));
            return os.GetObjectsQuery<Mail>(true)
                .Where(m =>
                    m.Direction == direction
                    && m.Account != null
                    && m.Account.Id == mailAccountId
                    && m.MessageId == messageId
                    && m.MessageIdHash == messageIdHash)
                .Select(m => new
                {
                    m.Id,
                    AccountId = m.Account!.Id,
                    m.FileName,
                    m.MessageId,
                    m.MessageIdHash
                })
                .Select(m => new MailInfo(m.Id, m.AccountId, m.FileName, m.MessageId, m.MessageIdHash))
                .ToList();
        }
    }

    internal static class HeaderListExtensions
    {
        internal static string ComputeHash(this HashAlgorithm hashAlgorithm, string input, Encoding encoding, string separator, int separatorByteCount)
        {
            _ = hashAlgorithm ?? throw new ArgumentNullException(nameof(hashAlgorithm));
            _ = input ?? throw new ArgumentNullException(nameof(input));
            _ = separator ?? throw new ArgumentNullException(nameof(separator));

            if (string.IsNullOrEmpty(separator))
            {
                throw new ArgumentException($"Invalid Separator: \"{separator}\"", nameof(separator));
            }

            if (separatorByteCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(separatorByteCount), "separatorByteCount <= 0");
            }

            var inputBytes = (encoding ?? Encoding.UTF8).GetBytes(input);
            var outputBytes = hashAlgorithm.ComputeHash(inputBytes);

            var sb = new StringBuilder(outputBytes.Length * 2 + 10);

            for (int i = 0, byteCounter = 0; i < outputBytes.Length; i++, byteCounter++)
            {
                sb.AppendFormat("{0:X2}", outputBytes[i]);
                if ((i % separatorByteCount) == separatorByteCount - 1 && i != outputBytes.Length - 1)
                {
                    sb.Append(separator);
                }
            }
            return sb.ToString();
        }

        internal static string ToSha256Hash(this HeaderList headerCollection)
        {
            var sb = new StringBuilder();

            foreach (var key in headerCollection)
            {
                sb.Append(key.Field);
                sb.Append(": ");
                sb.AppendLine(key.Value);
            }

            var r = sb.ToString().ToSha256Hash();

            return r;
        }

        internal static string ToSha256Hash(this string str)
        {
            using HashAlgorithm hash = SHA256.Create();
            var r = hash.ComputeHash(str, Encoding.ASCII, "-", 4);
            return r.ToString();

        }

        internal static string ToHMSGID(this HeaderList headerCollection)
            => $"<{headerCollection.ToSha256Hash()}_HMSGID>";

        internal static string ToNMSGID(this string str)
            => $"<{str.ToSha256Hash()}_NMSGID>".Replace("-", string.Empty);

        internal static string GetMessageIdHash(this HeaderList headerCollection)
        {
            var messageIdHeaderEntry = headerCollection["Message-ID"];

            if (string.IsNullOrEmpty(messageIdHeaderEntry))
            {
                messageIdHeaderEntry = headerCollection.ToHMSGID();
                return messageIdHeaderEntry;
            }

            var threadIndex = headerCollection["Thread-Index"];

            if (threadIndex is not null)
            {
                messageIdHeaderEntry = $"{threadIndex}|{messageIdHeaderEntry}";

                if (messageIdHeaderEntry.Length > 255)
                {
                    messageIdHeaderEntry = headerCollection.ToHMSGID();
                }
            }

            messageIdHeaderEntry = messageIdHeaderEntry.ToNMSGID();

            return messageIdHeaderEntry;
        }

        internal static string GetMessageId(this HeaderList headerCollection)
        {
            var messageIdHeaderEntry = headerCollection["Message-ID"];

            if (string.IsNullOrEmpty(messageIdHeaderEntry))
            {
                messageIdHeaderEntry = headerCollection.ToHMSGID();
                return messageIdHeaderEntry;
            }

            return messageIdHeaderEntry;
        }
    }
}
