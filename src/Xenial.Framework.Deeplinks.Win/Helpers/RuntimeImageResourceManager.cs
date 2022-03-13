using DevExpress.ExpressApp.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;

using Xenial.Framework.Deeplinks.Win.Helpers.Icons;

namespace Xenial.Framework.Deeplinks.Win.Helpers;

/// <summary>
/// 
/// </summary>
public sealed class RuntimeImageResourceManager
{
    private readonly string basePath;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="basePath"></param>
    public RuntimeImageResourceManager(string? basePath)
    {
        if (string.IsNullOrEmpty(basePath))
        {
            basePath = Path.GetTempPath();
        }

        this.basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageNames"></param>
    /// <returns></returns>
    public IDictionary<string, string> GenerateIcons(IEnumerable<string> imageNames!!)
        => new ReadOnlyDictionary<string, string>(GenerateIcons(imageNames, basePath));

    private static Dictionary<string, string> GenerateIcons(IEnumerable<string> imageNames, string basePath)
    {
        var images = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var imageName in imageNames)
        {
            var info = ImageLoader.Instance.GetImageInfo(imageName);
            if (info.IsEmpty)
            {
                continue;
            }

            var iconPath = Path.Combine(basePath, $"{imageName}.ico");
            var writer = new IconFileWriter();

            if (info.IsSvgImage)
            {
                var svgImage = info.CreateSvgImage();

                foreach (var imageSize in new[]
                {
                    new Size(32, 32),
                    new Size(64, 64)
                })
                {
                    // Get the target size according to the current DPI  
                    var scaleSize = ScaleUtils.GetScaleSize(imageSize);
                    // Generate raster image  
                    var render = svgImage.Render(scaleSize, SvgPaletteHelper.GetSvgPalette(UserLookAndFeel.Default, ObjectState.Normal), DefaultBoolean.False, DefaultBoolean.True);
                    AddImage(writer, render);
                }
            }
            else
            {
                AddImage(writer, info.Image);
                AddImage(writer, ImageLoader.Instance.GetLargeImageInfo(imageName).Image);
            }

            if (writer.Images.Count > 0)
            {

#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    //If we fail, something might be fishy anyway
                    writer.Save(iconPath);
                }
                catch { }
#pragma warning restore CA1031 // Do not catch general exception types

                if (File.Exists(iconPath))
                {
                    images[imageName] = iconPath;
                }
            }
        }

        return images;
    }

    private static void AddImage(IconFileWriter writer, Image image)
    {
        if (image is not null)
        {
            writer.AddImage(image);
        }
    }
}
