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
        public async partial Task<MyTarget> Execute(MyTarget myTarget) => throw new NotImplementedException();
    }

    public class FooController : ViewController
    {
        public FooController()
        {
            this.TargetViewType = ViewType.DetailView;
        }
    }
}
