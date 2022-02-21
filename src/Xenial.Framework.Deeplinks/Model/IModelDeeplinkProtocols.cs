using System;
using System.ComponentModel;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Xenial.Framework.Deeplinks.Generators;
using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
public interface IModelOptionsDeeplinkProtocols
{
    /// <summary>
    /// 
    /// </summary>
    IModelDeeplinkProtocols DeeplinkProtocols { get; }
}

/// <summary>
/// 
/// </summary>
[ImageName(XenialImages.Model_Protocols)]
[ModelNodesGenerator(typeof(ModelDeeplinkProtocolsGenerator))]
public interface IModelDeeplinkProtocols : IModelNode, IModelList<IModelDeeplinkProtocol>
{
    /// <summary>
    /// Indicates if protocols are handled at all
    /// </summary>
    [DefaultValue(true)]
    [Description("Indicates if protocols are handled at all")]
    bool EnableProtocols { get; set; }

    /// <summary>
    /// Indicates if the protocols will be automatically installed at startup
    /// </summary>
    [DefaultValue(true)]
    [Description("Indicates if the protocols will be automatically installed at startup")]
    bool AutoInstallProtocols { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Description("Defines the default protocol used to generate protocol links")]
    [DataSourceProperty("this")]
    IModelDeeplinkProtocol DefaultProtocol { get; set; }
}

/// <summary>
/// 
/// </summary>
[KeyProperty(nameof(ProtocolName))]
[ImageName(XenialImages.Model_Protocols)]
public interface IModelDeeplinkProtocol : IModelNode
{
    /// <summary>
    /// The name of the protocol. Should be in a valid Uri-Prefix format and should not start with a number.
    /// </summary>
    [Required]
    [Description("The name of the protocol. Should be in a valid Uri-Prefix format and should not start with a number.")]
    [Index(0)]
    [Category("Behavior")]
    //TODO: RegEx for correct protocol
    string ProtocolName { get; set; }

    /// <summary>
    /// End user hint about this protocol
    /// </summary>
    [Description("End user hint about this protocol")]
    string ProtocolDescription { get; set; }

    /// <summary>
    /// This displays the shape of the Uri handler prefix
    /// </summary>
    [Description("This displays the shape of the Uri handler prefix")]
    string ProtocolHandler { get; }
}

/// <summary>
/// 
/// </summary>
[DomainLogic(typeof(IModelDeeplinkProtocols))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
public static class ModelDeeplinksProtocolLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IModelDeeplinkProtocol? Get_DefaultProtocol(IModelDeeplinkProtocols node)
    {
        if (node is null)
        {
            return null;
        }

        return node.OrderBy(m => m.Index).FirstOrDefault();
    }
}
/// <summary>
/// 
/// </summary>
[DomainLogic(typeof(IModelDeeplinkProtocol))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
public static class ModelDeeplinkProtocolLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static string Get_ProtocolDescription(IModelDeeplinkProtocol option)
    {
        if (option == null || string.IsNullOrEmpty(option.ProtocolName))
        {
            return "Protocol Handler";
        }

        return $"{option.ProtocolName} Protocol";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static string Get_ProtocolHandler(IModelDeeplinkProtocol option)
    {
        if (option is null || string.IsNullOrEmpty(option.ProtocolName))
        {
            return string.Empty;
        }

        return $"{option.ProtocolName}://";
    }
}
