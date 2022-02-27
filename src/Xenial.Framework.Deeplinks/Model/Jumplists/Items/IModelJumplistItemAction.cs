using System.ComponentModel;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName("ModelEditor_Actions")]
[KeyProperty(nameof(ActionId))]
public interface IModelJumplistItemAction : IModelJumplistItemBase
{
    /// <summary>
    /// 
    /// </summary>
    [Browsable(false)]
    string ActionId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DataSourceProperty("Application.ActionDesign.Actions")] //TODO: Filter only by Window Controllers and root?
    [Required]
    IModelAction Action { get; set; }
}

/// <summary>
/// 
/// </summary>
[DomainLogic(typeof(IModelJumplistItemAction))]
[EditorBrowsable(EditorBrowsableState.Never)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By Convention")]
public static class ModelJumplistItemActionDomainLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelAction"></param>
    /// <returns></returns>
    public static IModelAction Get_Action(IModelJumplistItemAction modelAction!!)
        => modelAction.Application.ActionDesign.Actions[modelAction.ActionId];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelAction"></param>
    /// <param name="value"></param>
    public static void Set_Action(IModelJumplistItemAction modelAction!!, IModelAction value)
        => modelAction.ActionId = value?.Id ?? null!;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string? Get_Caption(IModelNode node)
    {
        if (node is IModelJumplistItemAction modelNode)
        {
            return modelNode.Action?.Caption;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string? Get_ImageName(IModelNode node)
    {
        if (node is IModelJumplistItemAction modelNode)
        {
            return modelNode.Action?.ImageName;
        }
        return null;
    }
}
