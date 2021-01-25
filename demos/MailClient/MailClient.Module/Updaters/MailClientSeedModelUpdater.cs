using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace MailClient.Module.Updaters
{
    public class MailClientSeedModelUpdater : ModuleUpdater
    {
        public MailClientSeedModelUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            ObjectSpace.EnsureSingletons();
        }
    }
}
