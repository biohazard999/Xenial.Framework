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
using Xenial.Framework;
using DevExpress.Utils.Filtering.Internal;
using System.Resources;

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

            string GetBaseFolder(int mailAccountId)
            {
                using var os = objectSpaceFactory(typeof(MailSettings));
                var settings = os.GetSingleton<MailSettings>();
                return Path.Combine(settings.StoragePath, mailAccountId.ToString());
            }

            (Guid uniqueId, string fileName) CreateFileName(int mailAccountId, MailDirection direction, DateTime dateTime)
            {
                var folder = GetBaseFolder(mailAccountId);
                const string ext = "eml";

                string GetDirection()
                  => direction switch
                  {
                      MailDirection.Inbound => "IN",
                      MailDirection.Outbound => "OUT",
                      _ => "UNKNOWN"
                  };

                var uuid = Guid.NewGuid();

                var fileName = $"{mailAccountId}_{GetDirection()}_{dateTime:yyyyMMddHHmmssfff}_{uuid}.{ext}";
                return (uuid, fileName);
            }

            (Guid uniqueId, string fileName) CreateFullFileName(int mailAccountId, MailDirection direction, DateTime dateTime)
            {
                var baseFolder = GetBaseFolder(mailAccountId);
                var (uniqueId, fileName) = CreateFileName(mailAccountId, direction, dateTime);
                var fullFileName = Path.Combine(baseFolder, dateTime.Year.ToString(), dateTime.Month.ToString(), fileName);
                return (uniqueId, fullFileName);
            }

            void EnsureDirectory(string fileName)
            {
                var directoryName = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }

            Mail CreateMail(
                IObjectSpace os,
                MailReceiveInfo mailReceiveInfo,
                string? folder
            )
            {
                var mail = os.CreateObject<Mail>();
                mail.Account = (MailAccount)os.GetObjectByKey(typeof(MailAccount), mailReceiveInfo.MailAccountId);
                mail.ReceivedDateTime = mailReceiveInfo.ReceivedDateTime;
                mail.UUId = mailReceiveInfo.UUId;
                mail.FileName = mailReceiveInfo.FileName;
                mail.Size = mailReceiveInfo.Size;
                mail.MessageId = mailReceiveInfo.MessageId;
                mail.MessageIdHash = mailReceiveInfo.MessageIdHash;
                mail.ImapFolderName = folder;
                mail.MessageDateTime = mailReceiveInfo.Message.Date.UtcDateTime;

                if (mail.MessageDateTime.HasValue && mail.MessageDateTime.Value <= DateTime.MinValue)
                {
                    mail.MessageDateTime = mailReceiveInfo.ReceivedDateTime;
                }

                mail.Subject = TruncateForIndex(mailReceiveInfo.Message.Subject);

                mail.To = TruncateForIndex(mailReceiveInfo.Message.To?.Mailboxes?.FirstOrDefault()?.Address);
                mail.From = TruncateForIndex(mailReceiveInfo.Message.From?.Mailboxes?.FirstOrDefault()?.Address);

                mail.ToAll = TruncateForIndex(string.Join(";", mailReceiveInfo.Message.To?.Select(t => t.ToString())));
                mail.FromAll = TruncateForIndex(string.Join(";", mailReceiveInfo.Message.From?.Select(t => t.ToString())));
                mail.CC = TruncateForIndex(string.Join(";", mailReceiveInfo.Message.Cc?.Select(t => t.ToString())));
                mail.BCC = TruncateForIndex(string.Join(";", mailReceiveInfo.Message.Bcc?.Select(t => t.ToString())));

                mail.MessagePriority = mailReceiveInfo.Message.Priority switch
                {
                    MessagePriority.Urgent => MailPriority.Urgent,
                    MessagePriority.NonUrgent => MailPriority.NonUrgent,
                    MessagePriority.Normal => MailPriority.Normal,
                    _ => MailPriority.Normal
                };

                mail.MessagePriorityX = mailReceiveInfo.Message.XPriority switch
                {
                    XMessagePriority.Lowest => MailPriorityX.Lowest,
                    XMessagePriority.Low => MailPriorityX.Low,
                    XMessagePriority.Normal => MailPriorityX.Normal,
                    XMessagePriority.High => MailPriorityX.High,
                    XMessagePriority.Highest => MailPriorityX.Highest,
                    _ => MailPriorityX.Normal,
                };

                mail.AttachmentCount = mailReceiveInfo.Message.Attachments?.Count() ?? null;

                return mail;
            }

            static string? TruncateForIndex(string? value)
            {
                static string? Truncate(string? value, int maxLength)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return value;
                    }

                    return value!.Length <= maxLength ? value : value.Substring(0, maxLength);
                }

                static string? TruncateByBytes(string? value, int maxBytes)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return value;
                    }

                    while (Encoding.Unicode.GetByteCount(value) > maxBytes)
                    {
                        if (value is null)
                        {
                            return null;
                        }
                        value = value.Substring(0, value.Length - 1);
                    }

                    return value;
                }


                var newValue = Truncate(value, Mail.TextSizeIndexable);
                newValue = TruncateByBytes(newValue, Mail.ByteSizeIndexable);
                return newValue;
            }

            async IAsyncEnumerable<Mail> ReceiveAsync(int mailAccountId, IMailFolder folder, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                await folder.OpenAsync(FolderAccess.ReadOnly, cancellationToken: cancellationToken);

                var uniqueIds = await folder.FetchAsync(0, -1, MessageSummaryItems.UniqueId, cancellationToken: cancellationToken);

                var query = new MailQuery(objectSpaceFactory);

                var existingMails = query.GetExistingInboundMails(mailAccountId);

                var inboxCount = uniqueIds.Count;
                for (var current = inboxCount - 1; current > -1; current--)
                {
                    using var headerStream = await folder.GetStreamAsync(uniqueIds[current].UniqueId, "HEADER", cancellationToken: cancellationToken);

                    var headerList = HeaderList.Load(headerStream, cancellationToken: cancellationToken);

                    if (current % 15 == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"{current}/{inboxCount} - {inboxCount - current + 1}");
                    }

                    var messageId = headerList.GetMessageId();
                    var messageIdHash = headerList.GetMessageIdHash();

                    var isInPreFetchQuery = existingMails.Any(m => m.MessageId == messageId && m.MessageIdHash == messageIdHash);
                    if (!isInPreFetchQuery)
                    {
                        var reQueryExistingMail = query.GetExistingInboundMails(mailAccountId, messageId, messageIdHash);

                        if (!reQueryExistingMail.Any())
                        {
                            var uniqueId = uniqueIds[current].UniqueId;
                            var currentDateTime = DateTime.UtcNow;

                            async Task<(Guid uuid, string fileName)> StoreMessage()
                            {
                                using (var rawMessageStream = await folder.GetStreamAsync(uniqueId, string.Empty, cancellationToken: cancellationToken))
                                {
                                    var (uuid, fullFileName) = CreateFullFileName(mailAccountId, MailDirection.Inbound, currentDateTime);

                                    EnsureDirectory(fullFileName);

                                    using (var fileStream = File.Create(fullFileName))
                                    {
                                        rawMessageStream.Seek(0, SeekOrigin.Begin);
                                        await rawMessageStream.CopyToAsync(fileStream);
                                    }
                                    return (uuid, fullFileName);
                                }
                            }

                            async Task<Mail?> LoadMesage(Guid uuid, string fullFileName)
                            {
                                using var fileStream = File.OpenRead(fullFileName);

                                var message = MimeMessage.Load(fileStream, cancellationToken: cancellationToken);

                                using var os = objectSpaceFactory(typeof(Mail));
                                try
                                {
                                    var mail = CreateMail(os, new(mailAccountId, currentDateTime, uuid, fullFileName, messageId, messageIdHash, message, fileStream.Length), folder.FullName);
                                    await CommitChangesAsync(os);
                                    return mail;
                                }
                                catch
                                {
                                    //TODO: HANDLE EXCEPTIOM
                                }
                                return null;
                            }

                            var (uuid, fileName) = await StoreMessage();

                            var mail = await LoadMesage(uuid, fileName);
                            if (mail is not null)
                            {
                                yield return mail;
                            }
                        }
                    }
                }
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

            try
            {
                await foreach (var mail in ReceiveAsync(mailAccount.Id, inbox, cancellationToken))
                {
                    yield return mail;
                }
            }
            finally
            {
                await imapClient.DisconnectAsync(true, cancellationToken);
            }

            yield break;
        }
    }

    internal record MailInfo(int Id, int MailAccountId, string? FileName, string MessageId, string MessageIdHash);

    internal record MailReceiveInfo(
        int MailAccountId,
        DateTime ReceivedDateTime,
        Guid UUId,
        string FileName,
        string MessageId,
        string MessageIdHash,
        MimeMessage Message,
        long Size
    );

    internal class MailQuery
    {
        private readonly Func<Type, IObjectSpace> objectSpaceFactory;
        public MailQuery(Func<Type, IObjectSpace> objectSpaceFactory)
            => this.objectSpaceFactory = objectSpaceFactory ?? throw new ArgumentNullException(nameof(objectSpaceFactory));

        public IList<MailInfo> GetExistingInboundMails(
          int mailAccountId
      ) => GetExistingMails(
          mailAccountId,
          MailDirection.Inbound
      );

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
          MailDirection direction
        )
        {
            using var os = objectSpaceFactory(typeof(Mail));
            return os.GetObjectsQuery<Mail>(true)
                .Where(m =>
                    m.Direction == direction
                    && m.Account != null
                    && m.Account.Id == mailAccountId)
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
