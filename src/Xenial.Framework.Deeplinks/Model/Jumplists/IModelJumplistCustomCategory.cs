using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
public interface IModelJumplistCustomCategory : IModelNode, IModelList<IModelJumplistItem>
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    string Caption { get; set; }
}
