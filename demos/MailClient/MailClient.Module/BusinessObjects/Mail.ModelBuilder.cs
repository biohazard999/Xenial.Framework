using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

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
                .HasCaption("Mail");

            For(m => m.MessageDateTime)
                .HasDisplayFormat("{0:G}");

            For(m => m.ReceivedDateTime)
                .HasDisplayFormat("{0:G}");
        }
    }
}
