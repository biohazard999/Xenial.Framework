using System;
using System.IO;

using DevExpress.Xpo.DB;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xenial.FeatureCenter.Module;
using Xenial.FeatureCenter.Module.Blazor;
using Xenial.FeatureCenter.Blazor.Server.Services;

namespace Xenial.FeatureCenter.Blazor.Server
{
    public partial class FeatureCenterBlazorApplication : BlazorApplication
    {
        static FeatureCenterBlazorApplication()
        {
            SQLiteConnectionProvider.Register();
            MySqlConnectionProvider.Register();
        }

        public FeatureCenterBlazorApplication()
        {
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;

            Modules.Add(new SystemModule());
            Modules.Add(new SystemBlazorModule());
            Modules.Add(new FeatureCenterModule());
            Modules.Add(new FeatureCenterBlazorModule());
        }

        protected override void OnSetupStarted()
        {
            base.OnSetupStarted();
#if DEBUG
            var dirName = Path.GetDirectoryName(GetType().Assembly.Location);
            var dbName = $"{nameof(FeatureCenterBlazorApplication)}.db";

            var dbPath = string.IsNullOrEmpty(dirName)
                ? dbName
                : Path.Combine(dirName, dbName);

            ConnectionString = SQLiteConnectionProvider.GetConnectionString(dbPath);
#else
            var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
            if (configuration.GetConnectionString("DefaultConnection") != null)
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection");
            }
#endif
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            _ = args ?? throw new ArgumentNullException(nameof(args));
            var dataStoreProvider = GetDataStoreProvider(args.ConnectionString, args.Connection);
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(dataStoreProvider, true));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }

        private IXpoDataStoreProvider GetDataStoreProvider(string connectionString, System.Data.IDbConnection connection)
        {
            var accessor = ServiceProvider.GetRequiredService<XpoDataStoreProviderAccessor>();
            lock (accessor)
            {
                if (accessor.DataStoreProvider == null)
                {
                    accessor.DataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, true);
                }
            }
            return accessor.DataStoreProvider;
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
