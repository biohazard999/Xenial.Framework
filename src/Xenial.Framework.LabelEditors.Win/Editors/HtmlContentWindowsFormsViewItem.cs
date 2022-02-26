using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;

using Xenial.Framework.LabelEditors.Model;

namespace Xenial.Framework.LabelEditors.Win.Editors;

/// <summary>
/// 
/// </summary>
public sealed class HtmlContentWindowsFormsViewItem : ViewItem
{
    /// <summary>
    /// 
    /// </summary>
    public IHtmlContentViewItem HtmlContentViewItem { get; }

    /// <summary>
    /// <para>Creates a new instance of the <see cref="HtmlContentWindowsFormsViewItem"/> class.</para>
    /// </summary>
    /// <param name="modelViewItem"></param>
    /// <param name="objectType">A <see cref="System.Type"/> object specifying the type of object for which the current View Item’s View is created.</param>
    public HtmlContentWindowsFormsViewItem(IHtmlContentViewItem modelViewItem, Type objectType)
        : base(objectType, modelViewItem?.Id)
    {
        _ = modelViewItem ?? throw new ArgumentNullException(nameof(modelViewItem));
        HtmlContentViewItem = modelViewItem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override object CreateControlCore()
    {
        var htmlContentControl = new HtmlContentControl();

        var collection = new SvgImageCollection();

        foreach (var imageSource in ImageLoader.Instance.ImageSources)
        {
            foreach (var imageName in imageSource.GetImageNames())
            {
                if (
                    HtmlContentViewItem.LoadAllImages
                    || (
                        HtmlContentViewItem.ImageNames is not null
                        && HtmlContentViewItem.ImageNames.Contains(imageName))
                    )
                {
                    var imageInfo = imageSource.FindImageInfo(imageName, true);
                    if (imageInfo.IsSvgImage)
                    {
                        var svgImage = new SvgImage(new MemoryStream(imageInfo.ImageBytes));
                        collection.Add(imageName, svgImage);
                    }
                }
            }
            if (imageSource is DevExpressImagesAssemblyImageSource devExpressImagesAssemblyImageSource)
            {
                var images = devExpressImagesAssemblyImageSource.GetImages(ImagePickerMode.SvgImages);
                foreach (var imageGroup in images)
                {
                    foreach (var imageWrapper in imageGroup.Value)
                    {
                        if (
                            HtmlContentViewItem.LoadAllImages
                            || (
                                HtmlContentViewItem.ImageNames is not null
                                && HtmlContentViewItem.ImageNames.Contains(imageWrapper.ImageName))
                            )
                        {
                            var imageInfo = imageSource.FindImageInfo(imageWrapper.ImageName, true);
                            if (imageInfo.IsSvgImage)
                            {
                                var svgImage = new SvgImage(new MemoryStream(imageInfo.ImageBytes));

                                collection.Add(imageWrapper.ImageName, svgImage);
                            }
                        }
                    }
                }
            }
        }

        htmlContentControl.HtmlImages = collection;

        htmlContentControl.HtmlTemplate
            .Template = HtmlContentViewItem.HtmlTemplate;

        htmlContentControl.HtmlTemplate
            .Styles = HtmlContentViewItem.CssStyles;

        htmlContentControl.DataContext = CurrentObject;

        htmlContentControl.ElementMouseClick += HtmlContentControl_ElementMouseClick;

        return htmlContentControl;
    }

    private void HtmlContentControl_ElementMouseClick(object sender, DevExpress.Utils.Html.DxHtmlElementMouseEventArgs e)
    {
        if (e.Element is not null && e.HitInfo.InLink)
        {
            //TODO: HandleLink
            using var _ = Process.Start(new ProcessStartInfo { FileName = e.HitInfo.Href, UseShellExecute = true, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unwireEventsOnly"></param>
    public override void BreakLinksToControl(bool unwireEventsOnly)
    {
        base.BreakLinksToControl(unwireEventsOnly);
        if (Control is not null)
        {
            Control.ElementMouseClick -= HtmlContentControl_ElementMouseClick;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnCurrentObjectChanged()
    {
        base.OnCurrentObjectChanged();
        if (Control is not null)
        {
            Control.DataContext = CurrentObject;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public new HtmlContentControl Control => (HtmlContentControl)base.Control;
}

