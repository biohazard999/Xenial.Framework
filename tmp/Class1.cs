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

    }

    [XenialAction(Caption = "Foo", ImageName = ImageNamesWithSizes.aac)]
    public partial record MyAction : IDetailViewAction<MyTarget>
    {
    }

    public class FooController : ViewController
    {
        public FooController()
        {
            this.TargetViewType = ViewType.DetailView;
        }
    }
}
