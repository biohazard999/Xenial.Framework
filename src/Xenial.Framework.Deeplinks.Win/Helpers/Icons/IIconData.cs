using System.Drawing;

namespace Xenial.Framework.Deeplinks.Win.Helpers.Icons;

/// <summary>The collection item for the <see cref="IconFileWriter.Images"/> property.</summary>
/// <remarks>I made this an interface so that it can easily be extended at the UI level. The
/// application adds a Modified property (to a struct that implements this interface) to keep track
/// of user edits.</remarks>
public interface IIconData
{
    /// <summary>
    /// 
    /// </summary>
    int BitDepth { get; set; }
    /// <summary>
    /// 
    /// </summary>
    Icon Icon { get; set; }
}
