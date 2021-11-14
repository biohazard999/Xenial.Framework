using System;

using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionContainers;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    /// <summary>
    /// 
    /// </summary>
    [XenialCheckLicense]
    public partial record NavigationOptions
    {
        /// <summary>   Gets or sets the startup navigation item identifier. </summary>
        ///
        /// <value> The startup navigation item identifier. </value>

        public string? StartupNavigationItemId { get; set; }

        /// <summary>   Gets or sets the startup navigation item. </summary>
        ///
        /// <value> The startup navigation item. </value>

        public Func<IModelNavigationItem, bool>? StartupNavigationItem { get; set; }

        /// <summary>   Gets or sets the default name of the parent image. </summary>
        ///
        /// <value> The default name of the parent image. </value>

        public string? DefaultParentImageName { get; set; }

        /// <summary>   Gets or sets the default name of the leaf image. </summary>
        ///
        /// <value> The default name of the leaf image. </value>

        public string? DefaultLeafImageName { get; set; }

        /// <summary>   Gets or sets the navigation style. </summary>
        ///
        /// <value> The navigation style. </value>

        public NavigationStyle? NavigationStyle { get; set; }

        /// <summary>   Gets or sets the default child items display style. </summary>
        ///
        /// <value> The default child items display style. </value>

        public ItemsDisplayStyle? DefaultChildItemsDisplayStyle { get; set; }

        /// <summary>   Gets or sets the show images. </summary>
        ///
        /// <value> The show images. </value>

        public bool? ShowImages { get; set; }
    }
}
