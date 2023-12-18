public class MyModule : DevExpress.ExpressApp.ModuleBase
{
    public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
    {
        base.AddGeneratorUpdaters(updaters);
        updaters.UseNoViewsGeneratorUpdater();
        updaters.UseDeclareViewsGeneratorUpdater();
        updaters.UseDetailViewLayoutBuilders();
        updaters.UseListViewColumnBuilders();
    }
}