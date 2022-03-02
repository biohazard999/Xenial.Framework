using System.ComponentModel;

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
    [DefaultValue(true)]
    bool EnableJumpList { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ModelBrowsable(typeof(ModelOptionsJumplistsVisibilityCalculator))]
    IModelJumplistTaskCategory TaskCategory { get; }

    /// <summary>
    /// 
    /// </summary>
    [ModelBrowsable(typeof(ModelOptionsJumplistsVisibilityCalculator))]
    IModelJumplistCustomCategories CustomCategories { get; }
}


/// <summary>
/// 
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ModelOptionsJumplistsVisibilityCalculator : IModelIsVisible
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public bool IsVisible(IModelNode node, string propertyName)
        => node?.GetValue<bool>(nameof(IModelJumplists.EnableJumpList)) ?? false;
}
