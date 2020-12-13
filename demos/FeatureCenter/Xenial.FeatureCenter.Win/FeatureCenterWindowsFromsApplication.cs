using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;

using Xenial.FeatureCenter.Module;
using Xenial.FeatureCenter.Module.Win;
using Xenial.Framework.TokenEditors;
using Xenial.Framework.TokenEditors.Win;
using Xenial.Framework.WebView.Win;

namespace Xenial.FeatureCenter.Win
{
    public class FeatureCenterWindowsFromsApplication : WinApplication
    {
        static FeatureCenterWindowsFromsApplication()
            => InMemoryDataStoreProvider.Register();

        public FeatureCenterWindowsFromsApplication()
        {
            ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            IgnoreUserModelDiffs = true;

            Modules.Add(new XenialTokenEditorsModule());
            Modules.Add(new XenialTokenEditorsWindowsFormsModule());

            Modules.Add(new XenialWebViewWindowsFormsModule());

            Modules.Add(new FeatureCenterModule());
            Modules.Add(new FeatureCenterWindowsFormsModule());
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(
                XPObjectSpaceProvider.GetDataStoreProvider(args.ConnectionString, args.Connection, true),
                true
            ));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }

        protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args)
        {
            args.Updater.Update();
            args.Handled = true;
            base.OnDatabaseVersionMismatch(args);
        }
    }
}
