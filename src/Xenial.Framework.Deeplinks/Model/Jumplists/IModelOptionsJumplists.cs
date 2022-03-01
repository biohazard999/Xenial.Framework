using System.ComponentModel;

using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
public interface IModelOptionsJumplists : IModelNode
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultValue(true)]
    [Category("Jumplists")]
    bool EnableJumpList { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ModelBrowsable(typeof(ModelOptionsJumplistsVisibilityCalculator))]
    IModelJumplists Jumplists { get; }
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
        => node?.GetValue<bool>(nameof(IModelOptionsJumplists.EnableJumpList)) ?? false;
}
