#if NET6_0_OR_GREATER

using System;
using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(Xenial.Framework.SystemModule.XenialHotReloadManager))]

namespace Xenial.Framework.SystemModule;

/// <summary>
/// For internal use only.
/// </summary>
public static class XenialHotReloadManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="types"></param>
    public static void ClearCache(Type[]? types)
    {
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <param name="types"></param>
    public static void UpdateApplication(Type[]? types)
    {
        foreach (var type in types ?? Array.Empty<Type>())
        {
            UpdateApp?.Invoke(null, new XenialHotReloadEventArgs(type));
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    public static event EventHandler<XenialHotReloadEventArgs>? UpdateApp;
}

/// <summary>
/// For internal use only.
/// </summary>
public sealed class XenialHotReloadEventArgs : EventArgs
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <param name="type"></param>
    public XenialHotReloadEventArgs(Type type)
        => Type = type;

    /// <summary>
    /// For internal use only.
    /// </summary>
    public Type Type { get; }
}

#endif
