using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.ModelBuilders;

namespace MailClient.Module.BusinessObjects
{
    public class MailModelBuilder : ModelBuilder<Mail>
    {
        public MailModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasNavigationItem("Mail - Mails")
                .HasImage("Glyph_Mail")
                .HasCaption("Mail")
                .WithDefaultListViewOptions(MasterDetailMode.ListViewAndDetailView)
            ;

            ForProperties(
                m => m.UUId,
                m => m.MessageIdHash,
                m => m.MessageId,
                m => m.FileName,
                m => m.MessageImportance,
                m => m.MessagePriority,
                m => m.MessagePriorityX,
                m => m.Size,
                m => m.ReceivedDateTime,
                m => m.Sent,
                m => m.AttachmentCount,
                m => m.ImapFolderName,
                m => m.HtmlBody,
                m => m.TextBody,
                m => m.BCC
            ).IsNotVisibleInAnyView();

            ForProperties(
                m => m.Direction,
                m => m.ImapFolderName,
                m => m.MessageDateTime,
                m => m.MessageId,
                m => m.MessageIdHash
            ).NotAllowingEdit();

            ForAllProperties()
                .Except(
                    m => m.Subject,
                    m => m.MessageDateTime
                )
            .IsNotVisibleInListView();

            ForPropertiesOfType<DateTime>()
                .HasDisplayFormat("{0:G}");

            ForProperties(
                m => m.From,
                m => m.CC,
                m => m.BCC
            ).UseTokenStringPropertyEditor()
             .NotAllowingEdit();

            For(m => m.ToAll)
                .UseTokenStringPropertyEditor(o => o.DropDownShowMode = TokenDropDownShowMode.Outlook);

            For(m => m.FromAll)
                .UseTokenStringPropertyEditor(o => o.DropDownShowMode = TokenDropDownShowMode.Default);
        }
    }
}
