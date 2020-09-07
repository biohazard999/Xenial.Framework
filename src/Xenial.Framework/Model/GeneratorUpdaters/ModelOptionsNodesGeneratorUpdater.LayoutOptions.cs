using DevExpress.Utils;

using Locations = DevExpress.Persistent.Base.Locations;

namespace Xenial.Framework.Model.GeneratorUpdaters
{

    /// <summary>
    /// Class LayoutOptions.
    /// </summary>
    public class LayoutOptions
    {
        /// <summary>
        /// Specifies a string used to separate layout items and their captions.            
        /// </summary>
        /// <value>The caption colon.</value>
        public string? CaptionColon { get; set; }

        /// <summary>
        /// Specifies whether a colon is used in captions displayed for View Items.
        /// </summary>
        /// <value><c>null</c> if [enable caption colon] contains no value, <c>true</c> if [enable caption colon]; otherwise, <c>false</c>.</value>
        public bool? EnableCaptionColon { get; set; }

        /// <summary>
        /// Specifies the default layout group and item captions location.
        /// </summary>
        /// <value>The caption location.</value>
        public Locations? CaptionLocation { get; set; }

        /// <summary>
        /// Specifies the default horizontal alignment of the layout group and item captions.
        /// </summary>
        /// <value>The caption horizontal alignment.</value>
        public HorzAlignment? CaptionHorizontalAlignment { get; set; }

        /// <summary>
        /// Specifies the default vertical alignment of layout group and item captions.
        /// </summary>
        /// <value>The caption vertical alignment.</value>
        public VertAlignment? CaptionVerticalAlignment { get; set; }

        /// <summary>
        /// Specifies the default layout group and item captions wrapping mode.
        /// </summary>
        /// <value>The caption word wrap.</value>
        public WordWrap? CaptionWordWrap { get; set; }

        /// <summary>
        /// Specifies whether images should be displayed for tabbed layout groups in a Detail View.
        /// </summary>
        /// <value><c>null</c> if [enable layout group images] contains no value, <c>true</c> if [enable layout group images]; otherwise, <c>false</c>.</value>
        public bool? EnableLayoutGroupImages { get; set; }
    }
}
