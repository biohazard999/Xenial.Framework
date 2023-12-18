
using DevExpress.ExpressApp;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using System;
using System.ComponentModel;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName("ModelEditor_Views")]
public interface IModelJumplistItemView : IModelJumplistItemBase
{
    /// <summary>
    /// 
    /// </summary>
    [DataSourceProperty("Application.Views")]
    [Required]
    [Category("Data")]
    [RefreshProperties(RefreshProperties.All)]
    IModelView View { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Description("Specifies the key property value (Oid by default) of the object that is present in the navigation control.")]
    [Category("Data")]
    [RefreshProperties(RefreshProperties.All)]
    string ObjectKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Category("Data")]
    [ModelBrowsable(typeof(ModelJumplistCreateObjectVisibilityCalculator))]
    bool CreateObject { get; set; }
}

/// <summary>
/// 
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ModelJumplistCreateObjectVisibilityCalculator : IModelIsVisible
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public bool IsVisible(IModelNode node, string propertyName)
    {
        if (node is IModelJumplistItemView modelJumplistItemView)
        {
            if (modelJumplistItemView.View is IModelDetailView && string.IsNullOrEmpty(modelJumplistItemView.ObjectKey))
            {
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// 
/// </summary>
[DomainLogic(typeof(IModelJumplistItemView))]
[EditorBrowsable(EditorBrowsableState.Never)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By Convention")]
public static class ModelJumplistItemViewDomainLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelView"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI-like return values should not be strings")]
    public static string Get_LaunchUri(IModelJumplistItemView modelView) => modelView switch
    {
        null => throw new ArgumentNullException(nameof(modelView)),
        _ => modelView.CreateObject
            ? $"{GetLaunchUri(modelView)}?createObject=true"
            : $"{GetLaunchUri(modelView)}{PrefixString('/', modelView.ObjectKey)}"
    };

    private static string GetLaunchUri(IModelJumplistItemView modelView) => modelView switch
    {
        null => throw new ArgumentNullException(nameof(modelView)),
        _ => $"{modelView.Protocol?.ProtocolName}://{DefaultDeeplinkVerbs.View}{PrefixString('/', modelView.View?.Id)}"
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
    public static string Get_Arguments(IModelJumplistItemView modelView)
        => modelView?.View is null
        ? $"verb={DefaultDeeplinkVerbs.View}"
        : $"verb={DefaultDeeplinkVerbs.View}&{new ViewShortcut(modelView.View.Id, modelView.ObjectKey)}";


    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool Get_CreateObject(IModelNode node)
    {
        if (node is IModelJumplistItemView modelNode)
        {
            if (string.IsNullOrEmpty(modelNode.ObjectKey))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string? Get_Caption(IModelNode node)
    {
        if (node is IModelJumplistItemView modelNode)
        {
            return modelNode.View?.Caption;
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
        if (node is IModelJumplistItemView modelNode)
        {
            return modelNode.View?.ImageName;
        }
        return null;
    }
}
