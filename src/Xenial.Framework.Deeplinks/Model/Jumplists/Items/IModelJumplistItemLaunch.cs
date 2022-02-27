namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
public interface IModelJumplistItemLaunch : IModelJumplistItemBase
{
    /// <summary>
    /// 
    /// </summary>
    //TODO: Path picker
    string PathToLaunch { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string Arguments { get; set; }

    /// <summary>
    /// 
    /// </summary>
    //TODO: Directory picker
    string WorkingDirectory { get; set; }
}
