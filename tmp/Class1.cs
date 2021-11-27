namespace MyProject
{
    [Xenial.XenialImageNames(
        Sizes = false,
        SmartComments = true,
        ResourceAccessors = true
    )]
    public partial class ImageNamesWithSizes
    {
        public ImageNamesWithSizes()
        {
            var foo = ImageNamesWithSizes.aac;
            
        }
    }

}
