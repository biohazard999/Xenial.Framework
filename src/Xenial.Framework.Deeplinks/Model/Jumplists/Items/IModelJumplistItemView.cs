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
[ImageName("ModelEditor_Views")]
public interface IModelJumplistItemView : IModelJumplistItemBase
{
    /// <summary>
    /// 
    /// </summary>
    [DataSourceProperty("Application.Views")] //TODO: Filter only by Window Controllers and root?
    [Required]
    [Category("Data")]
    IModelView View { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Description("Specifies the key property value (Oid by default) of the object that is present in the navigation control.")]
    [Category("Data")]
    string ObjectKey { get; set; }


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
    public static string Get_LaunchUri(IModelJumplistItemView modelView!!) =>
        $"{modelView.Protocol?.ProtocolName}://{DefaultDeeplinkVerbs.View}{PrefixString('/', modelView.View?.Id)}{PrefixString('/', modelView.ObjectKey)}";

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
    public static string Get_Arguments(IModelJumplistItemView modelView!!)
        => modelView.View is null
        ? $"verb={DefaultDeeplinkVerbs.View}"
        : $"verb={DefaultDeeplinkVerbs.View}&{new ViewShortcut(modelView.View.Id, modelView.ObjectKey)}";

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
