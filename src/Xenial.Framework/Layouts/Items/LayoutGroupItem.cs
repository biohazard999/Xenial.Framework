﻿using System;

using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;

using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.PubTernal;

using Locations = DevExpress.Persistent.Base.Locations;
using ToolTipIconType = DevExpress.Persistent.Base.ToolTipIconType;

namespace Xenial.Framework.Layouts.Items
{
    /// <summary>
    /// 
    /// </summary>
    [XenialCheckLicence]
    public partial record LayoutGroupItem
        : LayoutItem,
        ILayoutElementWithCaptionOptions,
        ILayoutElementWithCaption,
        ILayoutToolTip,
        ILayoutToolTipOptions,
        ILayoutGroupItem
    {
        private const FlowDirection defaultFlowDirection = FlowDirection.Vertical;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create()
            => new();

        /// <summary>
        /// Creates the specified nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(params LayoutItemNode[] nodes)
            => Create() with { Children = new(nodes) };

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption)
            => new(caption);

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, params LayoutItemNode[] nodes)
            => Create(caption) with { Children = new(nodes) };

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string imageName)
            => new(caption, imageName);

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string imageName, params LayoutItemNode[] nodes)
            => Create(caption, imageName) with { Children = new(nodes) };

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string imageName, string id)
            => new(caption, imageName, id);

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string? imageName, string id, FlowDirection flowDirection)
            => new(caption, imageName, id, flowDirection);

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string imageName, string id, params LayoutItemNode[] nodes)
            => Create(caption, imageName, id) with { Children = new(nodes) };

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, FlowDirection flowDirection)
            => new(caption, flowDirection);

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => Create(caption, flowDirection) with { Children = new(nodes) };

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string imageName, FlowDirection flowDirection)
            => new(caption, imageName, flowDirection);

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string imageName, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => Create(caption, imageName, flowDirection) with { Children = new(nodes) };

        /// <summary>
        /// Creates the specified caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <param name="nodes">The nodes.</param>
        /// <returns>Xenial.Framework.Layouts.Items.LayoutGroupItem.</returns>
        /// <autogeneratedoc />
        public static LayoutGroupItem Create(string caption, string? imageName, string id, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => Create(caption, imageName, id, flowDirection) with { Children = new(nodes) };

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGroupItem"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public LayoutGroupItem()
            => Direction = defaultFlowDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGroupItem"/> class.
        /// </summary>
        /// <param name="caption">The identifier.</param>
        /// <autogeneratedoc />
        public LayoutGroupItem(string caption)
            : this(caption, defaultFlowDirection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGroupItem"/> class.
        /// </summary>
        /// <param name="caption">The identifier.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <autogeneratedoc />
        public LayoutGroupItem(string caption, FlowDirection flowDirection)
            : this(caption, null, caption, flowDirection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGroupItem"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <autogeneratedoc />
        public LayoutGroupItem(string caption, string imageName)
            : this(caption, imageName, caption, defaultFlowDirection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGroupItem"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="id">The identifier.</param>
        /// <autogeneratedoc />
        public LayoutGroupItem(string caption, string imageName, string id)
            : this(caption, imageName, id, defaultFlowDirection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGroupItem"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <autogeneratedoc />
        public LayoutGroupItem(string caption, string? imageName, FlowDirection flowDirection)
            : this(caption, imageName, caption, flowDirection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGroupItem"/> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="flowDirection">The flow direction.</param>
        /// <autogeneratedoc />
        public LayoutGroupItem(string caption, string? imageName, string id, FlowDirection flowDirection)
        {
            Caption = caption;
            ShowCaption = !string.IsNullOrEmpty(caption);
            ImageName = imageName;
            Id = Slugifier.GenerateSlug(id);
            Direction = flowDirection;
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        /// <autogeneratedoc />
        public FlowDirection Direction { get; init; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>The name of the image.</value>
        /// <autogeneratedoc />
        public string? ImageName { get; set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        /// <autogeneratedoc />
        public string? Caption { get; set; }

        /// <summary>
        /// Gets or sets the show caption.
        /// </summary>
        /// <value>The show caption.</value>
        /// <autogeneratedoc />
        public bool? ShowCaption { get; set; }

        /// <summary>
        /// Gets or sets the caption horizontal alignment.
        /// </summary>
        /// <value>The caption horizontal align.</value>
        /// <autogeneratedoc />
        public HorzAlignment? CaptionHorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the caption location.
        /// </summary>
        /// <value>The caption location.</value>
        /// <autogeneratedoc />
        public Locations? CaptionLocation { get; set; }

        /// <summary>
        /// Gets or sets the caption vertical alignment.
        /// </summary>
        /// <value>The caption vertical align.</value>
        /// <autogeneratedoc />
        public VertAlignment? CaptionVerticalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the caption word wrap.
        /// </summary>
        /// <value>The caption word wrap.</value>
        /// <autogeneratedoc />
        public WordWrap? CaptionWordWrap { get; set; }

        /// <summary>
        /// Gets or sets the is collapsible group.
        /// </summary>
        /// <value>The is collapsible group.</value>
        /// <autogeneratedoc />
        public bool? IsCollapsibleGroup { get; set; }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        /// <autogeneratedoc />
        public string? ToolTip { get; set; }

        /// <summary>
        /// Gets or sets the type of the tool tip icon.
        /// </summary>
        /// <value>The type of the tool tip icon.</value>
        /// <autogeneratedoc />
        public ToolTipIconType? ToolTipIconType { get; set; }

        /// <summary>
        /// Gets or sets the tool tip title.
        /// </summary>
        /// <value>The tool tip title.</value>
        /// <autogeneratedoc />
        public string? ToolTipTitle { get; set; }

        /// <summary>
        /// Gets or sets the layout group options.
        /// </summary>
        /// <value>The layout group options.</value>
        /// <autogeneratedoc />
        public Action<IModelLayoutGroup>? LayoutGroupOptions { get; set; }
    }
}
