using System;
using System.IO;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;

using Xenial.FeatureCenter.Module;
using Xenial.FeatureCenter.Module.Win;
using Xenial.Framework.TokenEditors;
using Xenial.Framework.TokenEditors.Win;
using Xenial.Framework.WebView.Win;

namespace Xenial.FeatureCenter.Win
{
    public class FeatureCenterWindowsFormsApplication : WinApplication
    {
        static FeatureCenterWindowsFormsApplication()
            => SQLiteConnectionProvider.Register();

        public FeatureCenterWindowsFormsApplication()
        {
            IgnoreUserModelDiffs = true;

            var dirName = Path.GetDirectoryName(GetType().Assembly.Location);
            var dbName = $"{nameof(FeatureCenterWindowsFormsApplication)}.db";

            var dbPath = string.IsNullOrEmpty(dirName)
                ? dbName
                : Path.Combine(dirName, dbName);

            ConnectionString = SQLiteConnectionProvider.GetConnectionString(dbPath);
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;

            Modules.Add(new XenialTokenEditorsModule());
            Modules.Add(new XenialTokenEditorsWindowsFormsModule());

            Modules.Add(new XenialWebViewWindowsFormsModule());

            Modules.Add(new FeatureCenterModule());
            Modules.Add(new FeatureCenterWindowsFormsModule());
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
