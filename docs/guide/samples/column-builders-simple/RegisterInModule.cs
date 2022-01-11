using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;

namespace MyApplication.Module
{
    public sealed partial class MyApplicationModule : ModuleBase
    {
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.UseListViewColumnBuilders();
        }
    }
}
