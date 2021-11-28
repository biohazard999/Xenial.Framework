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

}
