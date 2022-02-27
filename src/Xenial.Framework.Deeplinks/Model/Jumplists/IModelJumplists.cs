using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
public interface IModelJumplists : IModelNode
{
    /// <summary>
    /// 
    /// </summary>
    IModelJumplistTaskCategory TaskCategory { get; }

    /// <summary>
    /// 
    /// </summary>
    IModelJumplistCustomCategories CustomCategories { get; }
}

