using System;
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
    [Category("Data")]
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI-like return values should not be strings")]
    public static string Get_LaunchUri(IModelJumplistItemAction modelAction) => modelAction switch
    {
        null => throw new ArgumentNullException(nameof(modelAction)),
        _ => $"{modelAction.Protocol?.ProtocolName}://{DefaultDeeplinkVerbs.Action}{PrefixString('/', modelAction.ActionId)}"
    };

    private static string PrefixString(char prefix, string? str)
    {
        str = str?.Trim('/');

        return string.IsNullOrEmpty(str)
            ? ""
            : $"{prefix}{str}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelAction"></param>
    /// <returns></returns>
    public static string Get_Arguments(IModelJumplistItemAction modelAction)
        => modelAction?.Action is null
        ? $"verb={DefaultDeeplinkVerbs.Action}"
        : $"verb={DefaultDeeplinkVerbs.Action}&actionId={modelAction.ActionId}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelAction"></param>
    /// <returns></returns>
    public static IModelAction Get_Action(IModelJumplistItemAction modelAction) => modelAction switch
    {
        null => throw new ArgumentNullException(nameof(modelAction)),
        _ => modelAction.Application.ActionDesign.Actions[modelAction.ActionId]
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelAction"></param>
    /// <param name="value"></param>
    public static void Set_Action(IModelJumplistItemAction modelAction, IModelAction value)
    {
        _ = modelAction ?? throw new ArgumentNullException(nameof(modelAction));
        modelAction.ActionId = value?.Id ?? null!;
    }

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
