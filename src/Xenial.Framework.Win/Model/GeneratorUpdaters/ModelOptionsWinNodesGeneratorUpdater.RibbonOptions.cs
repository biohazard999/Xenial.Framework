using DevExpress.XtraBars.Ribbon;

namespace Xenial.Framework.Win.Model.GeneratorUpdaters
{

    /// <summary>
    /// Class RibbonOptions.
    /// </summary>
    public record RibbonOptions
    {
        /// <summary>
        /// Specifies the style of the Ribbon UI when it is enabled via the DevExpress.ExpressApp.Win.SystemModule.IModelOptionsWin.FormStyle property.
        /// </summary>
        /// <value>The ribbon control style.</value>
        public RibbonControlStyle? RibbonControlStyle { get; set; }

        /// <summary>
        /// Indicates whether the Ribbon is minimized.
        /// </summary>
        /// <value><c>null</c> if [minimize ribbon] contains no value, <c>true</c> if [minimize ribbon]; otherwise, <c>false</c>.</value>
        public bool? MinimizeRibbon { get; set; }
    }
}
