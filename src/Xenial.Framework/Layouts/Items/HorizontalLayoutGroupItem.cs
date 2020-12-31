﻿
using DevExpress.ExpressApp.Layout;

namespace Xenial.Framework.Layouts.Items
{
    /// <summary>
    /// 
    /// </summary>
    [XenialCheckLicence]
    public partial record HorizontalLayoutGroupItem : LayoutGroupItem
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>Xenial.Framework.Layouts.Items.HorizontalLayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static new HorizontalLayoutGroupItem Create()
            => new();

        /// <summary>
        /// Creates the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Xenial.Framework.Layouts.Items.HorizontalLayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static new HorizontalLayoutGroupItem Create(string id)
            => new(id);

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalLayoutGroupItem"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public HorizontalLayoutGroupItem()
            => Direction = FlowDirection.Horizontal;

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalLayoutGroupItem"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <autogeneratedoc />
        public HorizontalLayoutGroupItem(string id)
            : base(id, FlowDirection.Horizontal) { }
    }
}
