using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ModelAbstractClass]
public interface IModelJumplistItemBase : IModelJumplistItem
{
    /// <summary>
    /// 
    /// </summary>
    [DataSourceProperty("Application.Options.DeeplinkProtocols")]
    //[Required(typeof(ProtocolRequiredCalculator))]
    [Category("Data")]
    [RefreshProperties(RefreshProperties.All)]
    IModelDeeplinkProtocol Protocol { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string Caption { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, DevExpress.Utils.ControlConstants.UITypeEditor)]
    string ImageName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    //[ModelBrowsable(typeof(ProtocolArgumentsVisibilityCalculator))]
    string Arguments { get; set; }

    /// <summary>
    /// 
    /// </summary>
    //[ModelBrowsable(typeof(ProtocolLaunchUriVisibilityCalculator))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings")]
    string LaunchUri { get; set; }
}

///// <summary>
///// 
///// </summary>
//[EditorBrowsable(EditorBrowsableState.Never)]
//public class ProtocolRequiredCalculator : IModelIsRequired
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="node"></param>
//    /// <param name="propertyName"></param>
//    /// <returns></returns>
//    public bool IsRequired(IModelNode node, string propertyName)
//        => node is IModelJumplistItemProtocol;
//}

///// <summary>
///// 
///// </summary>
//[EditorBrowsable(EditorBrowsableState.Never)]
//public class ProtocolArgumentsVisibilityCalculator : IModelIsVisible
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="node"></param>
//    /// <param name="propertyName"></param>
//    /// <returns></returns>
//    public bool IsVisible(IModelNode node, string propertyName)
//    {
//        if (node is IModelJumplistItemBase jumplistItemBase)
//        {
//            return jumplistItemBase.Protocol is null;
//        }
//        return false;
//    }
//}

///// <summary>
///// 
///// </summary>
//[EditorBrowsable(EditorBrowsableState.Never)]
//public class ProtocolLaunchUriVisibilityCalculator : IModelIsVisible
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="node"></param>
//    /// <param name="propertyName"></param>
//    /// <returns></returns>
//    public bool IsVisible(IModelNode node, string propertyName)
//    {
//        if (node is IModelJumplistItemBase jumplistItemBase)
//        {
//            return jumplistItemBase.Protocol is not null;
//        }
//        return false;
//    }
//}

/// <summary>
/// 
/// </summary>
//[DomainLogic(typeof(IModelJumplistItemBase))]
//[EditorBrowsable(EditorBrowsableState.Never)]
//[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By Convention")]
//public static class ModelJumplistItemBaseDomainLogic
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="modelProtocol"></param>
//    /// <returns></returns>
//    public static IModelDeeplinkProtocol Get_Protocol(IModelJumplistItemProtocol modelProtocol!!)
//    {
//        if (modelProtocol.Application.Options is IModelOptionsDeeplinkProtocols prot)
//        {
//            return prot.DeeplinkProtocols.DefaultProtocol;
//        }
//        return null!;
//    }
//}
