using System;
using System.Collections.Generic;
using System.Text;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
public abstract record ProtocolBuilder(string ProtocolName!!)
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "<Pending>")]
    public abstract string ToUri();
}

/// <summary>
/// 
/// </summary>
/// <param name="ProtocolName"></param>
/// <param name="Verb"></param>
public abstract record VerbProtocolBuilder(string ProtocolName!!, string Verb!!) : ProtocolBuilder(ProtocolName)
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToUri()
    {
        var sb = new StringBuilder();
        sb.Append($"{ProtocolName}://");
        sb.Append(Verb);
        sb.Append('/');
        BuildUriCore(sb);
        return sb.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    protected abstract void BuildUriCore(StringBuilder sb);
}

/// <summary>
/// 
/// </summary>
/// <param name="ViewId"></param>
/// <param name="ProtocolName"></param>
public record ViewProtocolBuilder(string ViewId!!, string ProtocolName!!)
    : VerbProtocolBuilder(ProtocolName, DefaultDeeplinkVerbs.View)
{
    /// <summary>
    /// 
    /// </summary>
    public string? ObjectKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? CreateObject { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    protected override void BuildUriCore(StringBuilder sb!!)
    {
        sb.Append(ViewId);

        if (string.IsNullOrEmpty(ObjectKey))
        {
            sb.Append($"/{ObjectKey}");
        }

        if (CreateObject.HasValue)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            sb.Append($"?createObject={CreateObject.Value.ToString().ToLowerInvariant()}");
#pragma warning restore CA1308 // Normalize strings to uppercase
        }
    }
}


/// <summary>
/// 
/// </summary>
/// <param name="ActionId"></param>
/// <param name="ProtocolName"></param>
public record ActionProtocolBuilder(string ActionId!!, string ProtocolName!!)
    : VerbProtocolBuilder(ProtocolName, DefaultDeeplinkVerbs.Action)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    protected override void BuildUriCore(StringBuilder sb!!)
        => sb.Append(ActionId);
}
