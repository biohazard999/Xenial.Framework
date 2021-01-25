using DevExpress.ExpressApp.DC;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.ModelBuilders;

namespace MailClient.Module.BusinessObjects
{
    public class MailAccountModelBuilder : ModelBuilder<MailAccount>
    {
        public MailAccountModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasNavigationItem("Settings - Accounts")
                .HasImage("Actions_Settings")
                .HasCaption("Account");
        }
    }
}
