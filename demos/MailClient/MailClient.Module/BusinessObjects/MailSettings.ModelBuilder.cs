using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

namespace MailClient.Module.BusinessObjects
{
    public class MailSettingsModelBuilder : ModelBuilder<MailSettings>
    {
        public MailSettingsModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }
        public override void Build()
        {
            base.Build();

            this.HasNavigationItem("Settings")
                .HasImage("Actions_Settings")
                .HasCaption("Settings")
                .IsSingleton(autoCommit: true)
                .GenerateNoListViews();
        }
    }
}
