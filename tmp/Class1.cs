using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;

using Xenial;

namespace MyProject
{
    [Xenial.XenialImageNames(
        Sizes = false,
        SmartComments = true,
        ResourceAccessors = true
    )]
    public static partial class ImageNamesWithSizes
    {
        static ImageNamesWithSizes()
        {
            var foo = ImageNamesWithSizes.aac;
            var x = ImageNamesWithSizes.AsImage.aac();

        }
    }

    public class MyTarget
    {
        public const string MyImage = "ABC";
    }

    [XenialAction(Caption = "Foo", ImageName = MyTarget.MyImage)]
    public partial class MyAction : IDetailViewAction<MyTarget>
    {
        public partial Task<(DetailView detailView, MyTarget newTarget)> Execute(MyTarget myTarget123) => throw new NotImplementedException();
    }

    public class FooController : ViewController
    {
        public FooController()
        {
            this.TargetViewType = ViewType.DetailView;

            SimpleAction x = new()
            {
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject
            };
            x.Execute += X_Execute;
        }

        private void X_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            e.
        }

        protected override void OnActivated() => base.OnActivated();

        protected override void OnDeactivated() => base.OnDeactivated();
    }
}
