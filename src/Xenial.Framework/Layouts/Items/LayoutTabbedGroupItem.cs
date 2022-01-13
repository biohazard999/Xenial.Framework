﻿using System;

using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;

using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.PubTernal;

using Locations = DevExpress.Persistent.Base.Locations;

#pragma warning disable CA1710 //Rename Type to end in Collection -> By Design
#pragma warning disable CA2227 //Collection fields should not have a setter: By Design

namespace Xenial.Framework.Layouts.Items;

/// <summary>   (Immutable) a layout tabbed group item. This record cannot be inherited. </summary>

[XenialCheckLicense]
public sealed partial record LayoutTabbedGroupItem : LayoutItem<LayoutTabGroupItem>,
    ILayoutElementWithCaption,
    ILayoutElementWithCaptionOptions
{
    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>Xenial.Framework.Layouts.Items.LayoutTabbedGroupItem.</returns>
    /// <autogeneratedoc />
    public static LayoutTabbedGroupItem Create()
        => new();

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>Xenial.Framework.Layouts.Items.LayoutTabbedGroupItem.</returns>
    /// <autogeneratedoc />
    public static LayoutTabbedGroupItem Create(params LayoutTabGroupItem[] tabs)
        => Create() with { Children = new(tabs) };

    /// <summary>
    /// Creates the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Xenial.Framework.Layouts.Items.LayoutTabbedGroupItem.</returns>
    /// <autogeneratedoc />
    public static LayoutTabbedGroupItem Create(string id)
        => new(id);

    /// <summary>
    /// Creates the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="tabs">The tabs.</param>
    /// <returns>Xenial.Framework.Layouts.Items.LayoutTabbedGroupItem.</returns>
    /// <autogeneratedoc />
    public static LayoutTabbedGroupItem Create(string id, params LayoutTabGroupItem[] tabs)
        => Create(id) with { Children = new(tabs) };

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutTabbedGroupItem"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public LayoutTabbedGroupItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutTabbedGroupItem"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <autogeneratedoc />
    public LayoutTabbedGroupItem(string id)
        => Id = Slugifier.GenerateSlug(id);

    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    /// <value>The direction.</value>
    /// <autogeneratedoc />
    public FlowDirection Direction { get; init; }

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
    /// Gets or sets the multi line.
    /// </summary>
    /// <value>The multi line.</value>
    /// <autogeneratedoc />
    public bool? MultiLine { get; set; }

    /// <summary>
    /// Gets or sets the tabbed group options.
    /// </summary>
    /// <value>The tabbed group options.</value>
    /// <autogeneratedoc />
    public Action<IModelTabbedGroup>? TabbedGroupOptions { get; set; }
}
