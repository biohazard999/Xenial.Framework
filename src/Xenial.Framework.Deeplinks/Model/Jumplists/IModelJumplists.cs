using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName(XenialImages.Model_Jumplists)]
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

