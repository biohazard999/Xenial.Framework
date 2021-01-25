using DevExpress.ExpressApp.DC;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.ModelBuilders;

namespace MailClient.Module.BusinessObjects
{
    public class MailSettingsModelBuilder : ModelBuilder<MailSettings>
    {
        public MailSettingsModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasNavigationItem("Settings - MailSettings")
                .HasImage("Actions_Settings");
        }
    }
}
