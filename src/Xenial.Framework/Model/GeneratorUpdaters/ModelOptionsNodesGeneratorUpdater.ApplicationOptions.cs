using System;

using DevExpress.ExpressApp;

namespace Xenial.Framework.Model.GeneratorUpdaters
{
    /// <summary>
    /// Class ApplicationOptions.
    /// </summary>
    [XenialCheckLicence]
    public partial record ApplicationOptions
    {
        /// <summary>
        /// Considered for reference properties that are displayed by a Lookup Property Editor<br/>
        ///  in the DevExpress.Persistent.Base.LookupEditorMode.Auto mode (see
        ///  DevExpress.ExpressApp.Model.IModelCommonMemberViewItem.LookupEditorMode).<br/>
        ///  If the assumed object count in the Lookup Property Editor's data source collection<br/>
        ///  is greater than this property value, none of these objects is retrieved, and<br/>
        ///  the Search feature is available.
        /// </summary>
        ///
        /// <value> The lookup small collection item count. </value>

        public int? LookupSmallCollectionItemCount { get; set; }

        /// <summary>
        /// Specifies the default mode used to access the collection of business objects<br/>
        /// in List Views when the DevExpress.ExpressApp.Model.IModelListView.DataAccessMode<br/>
        /// property is not initialized.<br/>
        /// </summary>
        ///
        /// <value> The data access mode. </value>

        public CollectionSourceDataAccessMode? DataAccessMode { get; set; }

        private LayoutOptions layout = new LayoutOptions();

        /// <summary>   Gets or sets the layout options. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <value> The layout. </value>

        public LayoutOptions Layout
        {
            get => layout;
            set => layout = value ?? throw new ArgumentNullException(nameof(Layout));
        }
    }
}
