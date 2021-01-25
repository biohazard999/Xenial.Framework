using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using MailClient.Module.BusinessObjects;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

using MimeKit;

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

            var inbox = imapClient.Inbox;

            await foreach (var mail in ReceiveAsync(inbox, cancellationToken))
            {

            }


            yield break;
        }

        async IAsyncEnumerable<Mail> ReceiveAsync(IMailFolder folder, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
                using var os = objectSpaceFactory(typeof(Mail));


                yield return (Mail)null;
            }
        }
    }

    internal static class HeaderListExtensions
    {
        public static string ComputeHash(this HashAlgorithm hashAlgorithm, string input, Encoding encoding, string separator, int separatorByteCount)
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

        public static string ToSha256Hash(this HeaderList headerCollection)
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

        public static string ToSha256Hash(this string str)
        {
            using HashAlgorithm hash = SHA256.Create();
            var r = hash.ComputeHash(str, Encoding.ASCII, "-", 4);
            return r.ToString();

        }
        public static string ToHMSGID(this HeaderList headerCollection)
            => $"<{headerCollection.ToSha256Hash()}_HMSGID>";

        public static string ToNMSGID(this string str)
            => $"<{str.ToSha256Hash()}_NMSGID>".Replace("-", string.Empty);

        public static string GetMessageIdHash(this HeaderList headerCollection)
        {
            var messageIdHeaderEntry = headerCollection["Message-ID"];

            if (string.IsNullOrEmpty(messageIdHeaderEntry))
            {
                messageIdHeaderEntry = headerCollection.ToHMSGID();
                return messageIdHeaderEntry;
            }

            if (headerCollection["Thread-Index"] != null)
            {
                messageIdHeaderEntry = headerCollection["Thread-Index"] + "|" + messageIdHeaderEntry;

                if (messageIdHeaderEntry.Length > 255)
                {
                    messageIdHeaderEntry = headerCollection.ToHMSGID();
                }
            }

            messageIdHeaderEntry = messageIdHeaderEntry.ToNMSGID();

            return messageIdHeaderEntry;
        }

        public static string GetMessageId(this HeaderList headerCollection)
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
