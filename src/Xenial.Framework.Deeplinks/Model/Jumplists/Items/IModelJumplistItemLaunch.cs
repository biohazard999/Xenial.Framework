using DevExpress.Persistent.Base;

using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName(XenialImages.Model_Protocol_Launch)]
public interface IModelJumplistItemLaunch : IModelJumplistItemBase
{
    /// <summary>
    /// 
    /// </summary>
    //TODO: Path picker
    string ProcessPath { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string ProcessArguments { get; set; }

    /// <summary>
    /// 
    /// </summary>
    //TODO: Directory picker
    string WorkingDirectory { get; set; }
}
