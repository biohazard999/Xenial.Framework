using System;

using DevExpress.ExpressApp;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    /// <summary>
    /// Class ApplicationOptions.
    /// </summary>
    [XenialCheckLicence]
    public partial record AppOptions
    {
        /// <summary>   Gets or sets the title. </summary>
        ///
        /// <value> The title. </value>

        public string? Title { get; set; }

        /// <summary>   Gets or sets the about information string. </summary>
        ///
        /// <value> The about information string. </value>

        public string? AboutInfoString { get; set; }

        /// <summary>   Gets or sets the protected content text. </summary>
        ///
        /// <value> The protected content text. </value>

        public string? ProtectedContentText { get; set; }

        /// <summary>   Gets or sets the preferred language. </summary>
        ///
        /// <value> The preferred language. </value>

        public string? PreferredLanguage { get; set; }

        /// <summary>   Gets or sets the version format. </summary>
        ///
        /// <value> The version format. </value>

        public string? VersionFormat { get; set; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        public string? Description { get; set; }

        /// <summary>   Gets or sets the logo. </summary>
        ///
        /// <value> The logo. </value>

        public string? Logo { get; set; }

        /// <summary>   Gets or sets the copyright. </summary>
        ///
        /// <value> The copyright. </value>

        public string? Copyright { get; set; }

        /// <summary>   Gets or sets the company. </summary>
        ///
        /// <value> The company. </value>

        public string? Company { get; set; }
    }
}
