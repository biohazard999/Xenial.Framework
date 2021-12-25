using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.Win;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Xpo;

using MailClient.Module;
using MailClient.Module.Win;

namespace MailClient.Win
{
    public class MailClientWindowsFormsApplication : WinApplication
    {
        public MailClientWindowsFormsApplication()
        {
            IgnoreUserModelDiffs = true;
            ConnectionString = DevExpress.Xpo.DB.MSSqlConnectionProvider.GetConnectionString(@"MANUEL-SURFACE-\SQLEXPRESS", "MailClient");
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;

            Modules.AddRange(new ModuleBase[]
            {
                new SystemModule(),
                new SystemWindowsFormsModule(),
                new ValidationModule(),
                new ValidationWindowsFormsModule(),
                new MailClientModule(),
                new MailClientWindowsFormsModule()
            });
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            _ = args ?? throw new ArgumentNullException(nameof(args));
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(
                XPObjectSpaceProvider.GetDataStoreProvider(args.ConnectionString, args.Connection, true),
                true
            ));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }

        protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args)
        {
            _ = args ?? throw new ArgumentNullException(nameof(args));
            args.Updater.Update();
            args.Handled = true;
            base.OnDatabaseVersionMismatch(args);
        }
    }
}
