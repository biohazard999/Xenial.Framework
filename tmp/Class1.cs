using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;

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

    [Xenial.XenialAction]
    public partial record MyAction
    {

    }

    public class FooController : ViewController
    {
        public FooController()
        {
        }
    }
}
