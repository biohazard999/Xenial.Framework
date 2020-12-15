using System;

using DevExpress.Xpo.DB;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.SystemModule;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;

using Microsoft.Extensions.DependencyInjection;

using Xenial.FeatureCenter.Module;
using Xenial.FeatureCenter.Module.Blazor;
using Xenial.FeatureCenter.Blazor.Server.Services;

namespace Xenial.FeatureCenter.Blazor.Server
{
    public partial class FeatureCenterBlazorApplication : BlazorApplication
    {
        static FeatureCenterBlazorApplication()
              => SQLiteConnectionProvider.Register();

        public FeatureCenterBlazorApplication()
        {
            ConnectionString = SQLiteConnectionProvider.GetConnectionString(nameof(FeatureCenterBlazorApplication));
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;

            Modules.Add(new SystemModule());
            Modules.Add(new SystemBlazorModule());
            Modules.Add(new FeatureCenterModule());
            Modules.Add(new FeatureCenterBlazorModule());
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
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
            args.Updater.Update();
            args.Handled = true;
            base.OnDatabaseVersionMismatch(args);
        }
    }
}
