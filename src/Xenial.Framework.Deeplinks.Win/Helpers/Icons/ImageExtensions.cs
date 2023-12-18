using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Xenial.Framework.Deeplinks.Win.Helpers.Icons;

/// <summary>Several synchronous and asynchronous extension methods
/// to make saving and converting images a little easier.</summary>
public static class ImageExtensions
{
    /// <summary>Cached encoder for improved performance.</summary>
    private static readonly Lazy<ImageCodecInfo> jpegEncoder = new Lazy<ImageCodecInfo>(() => ImageCodecInfo.GetImageDecoders().FirstOrDefault(j => j.FormatID == ImageFormat.Jpeg.Guid));

    internal static ImageCodecInfo JpegEncoder => jpegEncoder.Value;

    /// <summary>Creates a 24 bit-per-pixel copy of the source image.</summary>
    public static Image CopyImage(this Image image)
        => CopyImage(image, PixelFormat.Format24bppRgb);

    /// <summary>Creates a copy of the source image with the specified pixel format.</summary><remarks>
    /// This can also be achieved with the <see cref="System.Drawing.Bitmap.Clone(Rectangle, PixelFormat)"/>
    /// overload, but I have had issues with that method.</remarks>
    public static Image CopyImage(this Image image, PixelFormat format)
    {
        // Don't try to draw a new Bitmap with an indexed pixel format.
        if (image is Bitmap bitmap &&
            (format == PixelFormat.Format1bppIndexed
                || format == PixelFormat.Format4bppIndexed
                || format == PixelFormat.Format8bppIndexed
                || format == PixelFormat.Indexed
            )
        )
        {
            return bitmap.Clone(new Rectangle(0, 0, image.Width, image.Height), format);
        }

        var result = new Bitmap(image.Width, image.Height, format);

        using var graphics = Graphics.FromImage(result);

        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.CompositingQuality = CompositingQuality.HighQuality;

        graphics.DrawImage(image, 0, 0, result.Width, result.Height);

        return result;
    }

    /// <summary>Resizes an image, optionally maintaining width:height ratios.</summary>
    /// <param name="image">The <see cref="Image"/> that you wish to resize.</param>
    /// <param name="width">The desired width of the resulting image.</param>
    /// <param name="height">The desired height of the resulting image.</param>
    /// <param name="maintainAspectRatio"><b>True</b> to maintain aspect ratio,
    /// otherwise <b>false</b>. This defaults to <b>true</b>.</param>
    /// <returns>The resulting resized <see cref="Image"/> object.</returns>
    public static Image Resize(this Image image, int width, int height, bool maintainAspectRatio = true)
    {
        if (maintainAspectRatio)
        {
            // calculate resize ratio
            var ratio = (double)width / image.Width;

            if (ratio * image.Height > height)
            {
                ratio = (double)height / image.Height;
            }

            width = (int)Math.Round(ratio * image.Width, MidpointRounding.AwayFromZero);
            height = (int)Math.Round(ratio * image.Height, MidpointRounding.AwayFromZero);
        }

        using var bmp = new Bitmap(width, height);

        using var g = Graphics.FromImage(bmp);

        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.DrawImage(image, 0, 0, width, height);

        return bmp;
    }

    /// <summary>Convert an image to a byte array in png format.</summary>
    public static byte[] ToArray(this Image image)
    {
        using var stream = new MemoryStream();

        using var img = image.CopyImage(PixelFormat.Format32bppArgb);
        img.Save(stream, ImageFormat.Png);

        stream.Position = 0;

        return stream.ToArray();
    }
}
