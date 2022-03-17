﻿using System;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable")]
public sealed class ViewNotFoundException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public string ViewId { get; } = "";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewId"></param>
    public ViewNotFoundException(string viewId) : base($"Could not find View with Id='{viewId}'")
        => ViewId = viewId;

    /// <summary>
    /// 
    /// </summary>
    public ViewNotFoundException() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ViewNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}