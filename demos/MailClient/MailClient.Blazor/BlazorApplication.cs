
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.SystemModule;

using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;

using MailClient.Blazor.Services;
using MailClient.Module;
using MailClient.Module.Blazor;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

namespace MailClient.Blazor
{
    public class MailClientBlazorApplication : BlazorApplication
    {
        static MailClientBlazorApplication()
            => DevExpress.Xpo.DB.MySqlConnectionProvider.Register();

        public MailClientBlazorApplication()
        {
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;

            Modules.Add(new SystemModule());
            Modules.Add(new SystemBlazorModule());
            Modules.Add(new MailClientModule());
            Modules.Add(new MailClientBlazorModule());
        }

        protected override void OnSetupStarted()
        {
            base.OnSetupStarted();
#if DEBUG
            ConnectionString = DevExpress.Xpo.DB.MySqlConnectionProvider.GetConnectionString("localhost", "root", "root", "MailClient");
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
