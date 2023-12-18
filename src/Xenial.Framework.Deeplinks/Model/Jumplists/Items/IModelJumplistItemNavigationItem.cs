using System;
using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName("ModelEditor_Navigation_Items")]
public interface IModelJumplistItemNavigationItem : IModelJumplistItemBase
{
    /// <summary>
    /// 
    /// </summary>
    [DataSourceProperty("Application.NavigationItems.AllItems")]
    [Required]
    IModelNavigationItem NavigationItem { get; set; }
}

/// <summary>
/// 
/// </summary>
[DomainLogic(typeof(IModelJumplistItemNavigationItem))]
[EditorBrowsable(EditorBrowsableState.Never)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By Convention")]
public static class ModelJumplistItemNavigationItemDomainLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelView"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI-like return values should not be strings")]
    public static string Get_LaunchUri(IModelJumplistItemNavigationItem modelView) => modelView switch
    {
        null => throw new ArgumentNullException(nameof(modelView)),
        _ => $"{modelView.Protocol?.ProtocolName}://{DefaultDeeplinkVerbs.View}{PrefixString('/', modelView.NavigationItem?.View?.Id)}{PrefixString('/', modelView.NavigationItem?.ObjectKey)}"
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
    /// <param name="modelView"></param>
    /// <returns></returns>
    public static string Get_Arguments(IModelJumplistItemNavigationItem modelView)
        => modelView?.NavigationItem?.View is null
        ? $"verb={DefaultDeeplinkVerbs.View}"
        : $"verb={DefaultDeeplinkVerbs.View}&{new ViewShortcut(modelView.NavigationItem.View.Id, modelView.NavigationItem.ObjectKey)}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string? Get_Caption(IModelNode node)
    {
        if (node is IModelJumplistItemNavigationItem modelNode)
        {
            return modelNode.NavigationItem?.Caption;
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
        if (node is IModelJumplistItemNavigationItem modelNode)
        {
            return modelNode.NavigationItem?.ImageName;
        }
        return null;
    }
}
