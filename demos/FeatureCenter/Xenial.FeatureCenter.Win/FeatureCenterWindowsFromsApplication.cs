
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

using System;

using Xenial.FeatureCenter.Module;
using Xenial.FeatureCenter.Module.Win;

namespace Xenial.FeatureCenter.Win
{
    public class FeatureCenterWindowsFromsApplication : WinApplication
    {
        public FeatureCenterWindowsFromsApplication()
        {
            Modules.Add(new FeatureCenterModule());
            Modules.Add(new FeatureCenterWindowsFormsModule());
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
            => args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
    }
}
