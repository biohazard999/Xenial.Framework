using System;

using DevExpress.ExpressApp;
using DevExpress.XtraBars.Ribbon;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Win.Model.GeneratorUpdaters
{
    /// <summary>
    /// Class ApplicationWinOptions.
    /// Implements the <see cref="Xenial.Framework.Model.GeneratorUpdaters.ApplicationOptions" />
    /// </summary>
    /// <seealso cref="Xenial.Framework.Model.GeneratorUpdaters.ApplicationOptions" />
    public class ApplicationWinOptions : ApplicationOptions
    {
        /// <summary>
        /// Specifies whether the Standard UI or Ribbon UI is used in the Windows Forms Application.
        /// </summary>
        /// <value>The form style.</value>
        public RibbonFormStyle? FormStyle { get; set; }

        /// <summary>
        /// Provides access to the RibbonOptions child node.
        /// </summary>
        /// <value>The ribbon options.</value>
        public RibbonOptions RibbonOptions { get; set; } = new RibbonOptions();

        /// <summary>
        /// Specifies the kind of a Window in which new Views should be invoked, when a multiple document interface is used.
        /// </summary>
        /// <value>The MDI default new window target.</value>
        public NewWindowTarget? MdiDefaultNewWindowTarget { get; set; }

        /// <summary>
        /// Specifies the type of messaging used in Windows Forms application.
        /// </summary>
        /// <value>The messaging.</value>
        public Type? Messaging { get; set; }

        /// <summary>
        ///  Specifies the Show View Strategy (see DevExpress.ExpressApp.ShowViewStrategyBase) used in the WinForms application.
        /// </summary>
        /// <value>The type of the UI.</value>
        public UIType? UIType { get; set; }

        /// <summary>
        /// Specifies whether a business object's icon should be displayed in a tab in a WinForms application with the TabbedMDI UI type.
        /// </summary>
        /// <value><c>null</c> if [show tab image] contains no value, <c>true</c> if [show tab image]; otherwise, <c>false</c>.</value>
        public bool? ShowTabImage { get; set; }

        /// <summary>
        /// Specifies whether the HTML formatting of UI text elements is allowed.
        /// </summary>
        /// <value><c>null</c> if [enable HTML formatting] contains no value, <c>true</c> if [enable HTML formatting]; otherwise, <c>false</c>.</value>
        public bool? EnableHtmlFormatting { get; set; }
    }
}
