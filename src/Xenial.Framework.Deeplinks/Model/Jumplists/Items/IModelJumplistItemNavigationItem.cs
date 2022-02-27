using System.ComponentModel;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
public interface IModelJumplistItemNavigationItem : IModelJumplistItem
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
