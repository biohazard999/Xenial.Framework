using System.ComponentModel;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

using Xenial.Framework.Images;

namespace Xenial.Framework.Deeplinks.Model;

/// <summary>
/// 
/// </summary>
[ImageName(XenialImages.Model_Protocol)]
public interface IModelJumplistItemProtocol : IModelJumplistItemBase
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Category("Data")]
    //TODO: Editor for default verbs
    string Verb { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Category("Data")]
    string Route { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Category("Data")]
    string Query { get; set; }
}

/// <summary>
/// 
/// </summary>
[DomainLogic(typeof(IModelJumplistItemProtocol))]
[EditorBrowsable(EditorBrowsableState.Never)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "By Convention")]
public static class ModelJumplistItemProtocolDomainLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelProtocol"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI-like return values should not be strings")]
    public static string Get_LaunchUri(IModelJumplistItemProtocol modelProtocol!!) =>
        $"{modelProtocol.Protocol?.ProtocolName}://{modelProtocol.Verb?.Trim('/')}{PrefixString('/', modelProtocol.Route)}{PrefixString('?', modelProtocol.Query)}";

    private static string PrefixString(char prefix, string? str)
    {
        str = str?.Trim('/');

        return string.IsNullOrEmpty(str)
            ? ""
            : $"{prefix}{str}";
    }
}
