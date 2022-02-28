﻿using System.ComponentModel;

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
    [DataSourceProperty("Application.Options.DeeplinkProtocols")] //TODO: Filter only by Window Controllers and root?
    [Required]
    [Category("Data")]
    IModelDeeplinkProtocol Protocol { get; set; }
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
    public static IModelDeeplinkProtocol Get_Protocol(IModelJumplistItemProtocol modelProtocol!!)
    {
        if (modelProtocol.Application.Options is IModelOptionsDeeplinkProtocols prot)
        {
            return prot.DeeplinkProtocols.DefaultProtocol;
        }
        return null!;
    }
}
