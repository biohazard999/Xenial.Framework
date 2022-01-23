using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Layouts.Items.LeafNodes;

/// <summary>   (Immutable) a layout static image item. </summary>
[XenialCheckLicense]
[XenialModelOptions(
    typeof(IModelStaticImage), IgnoredMembers = new[]
    {
        nameof(IModelStaticImage.Id),
        nameof(IModelStaticImage.Index),
        nameof(IModelStaticImage.ImageName),
        nameof(IModelViewItem.Caption)
    }
)]
public partial record LayoutStaticImageItem : LayoutViewItem
{
    /// <summary>   Gets the name of the image. </summary>
    ///
    /// <value> The name of the image. </value>

    public string ImageName { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutStaticImageItem"/> class.
    /// </summary>
    ///
    /// <param name="imageName">    Name of the image. </param>

    public LayoutStaticImageItem(string imageName)
        : base(imageName)
            => ImageName = imageName;

    /// <summary>   Gets or sets the image options. </summary>
    ///
    /// <value> The image options. </value>

    public Action<IModelStaticImage>? ImageOptions { get; set; }
}
