using System;

using Xenial.Framework.Deeplinks.Generators;

namespace DevExpress.ExpressApp.Model.Core;

/// <summary>
/// 
/// </summary>
public static class XenialDeeplinksGeneratorExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="updaters"></param>
    /// <param name="options"></param>
    /// <param name="protocols"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ModelNodesGeneratorUpdaters UseXenialDeeplinks(
        this ModelNodesGeneratorUpdaters updaters,
        ModelDeeplinkProtocols? options = null,
        params ModelDeeplinkProtocol[] protocols
    )
    {
        _ = updaters ?? throw new ArgumentNullException(nameof(updaters));

        options = options ?? new();

        //Add the protocols before the options so we can have a default protocol
        updaters.Add(new ModelDeepLinkGeneratorUpdaters(protocols));
        updaters.Add(new ModelDeepLinkOptionsGeneratorUpdaters(options));

        return updaters;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="updaters"></param>
    /// <param name="protocols"></param>
    /// <returns></returns>
    public static ModelNodesGeneratorUpdaters UseXenialDeeplinks(
        this ModelNodesGeneratorUpdaters updaters,
        params ModelDeeplinkProtocol[] protocols
    ) => updaters.UseXenialDeeplinks(null, protocols);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="updaters"></param>
    /// <param name="protocol"></param>
    /// <returns></returns>
    public static ModelNodesGeneratorUpdaters UseXenialDeeplinks(
        this ModelNodesGeneratorUpdaters updaters,
        ModelDeeplinkProtocol protocol
    ) => updaters.UseXenialDeeplinks(null, new[] { protocol });


    /// <summary>
    /// 
    /// </summary>
    /// <param name="updaters"></param>
    /// <returns></returns>
    public static ModelNodesGeneratorUpdaters UseXenialDeeplinks(
        this ModelNodesGeneratorUpdaters updaters
    ) => updaters.UseXenialDeeplinks(null, Array.Empty<ModelDeeplinkProtocol>());
}
