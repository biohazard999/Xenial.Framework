using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.Utils;

public static class LogHelper
{
    public static IDisposable LogInformationTick(this ILogger logger, string? message, params object?[] args)
    {
        return new Ticker(
        _ =>
        {
            logger.LogInformation($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogInformation($"[END]: {w.Elapsed} {message}", args);
        });
    }

    public static IDisposable LogWarningTick(this ILogger logger, string? message, params object?[] args)
    {
        return new Ticker(
        _ =>
        {
            logger.LogWarning($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogWarning($"[END]: {w.Elapsed} {message}", args);
        });
    }


    public static IDisposable LogErrorTick(this ILogger logger, string? message, params object?[] args)
    {
        return new Ticker(
        _ =>
        {
            logger.LogError($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogError($"[END]: {w.Elapsed} {message}", args);
        });
    }

    public static IDisposable LogDebugTick(this ILogger logger, string? message, params object?[] args)
    {
        return new Ticker(
        _ =>
        {
            logger.LogDebug($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogDebug($"[END]: {w.Elapsed} {message}", args);
        });
    }

    public static IDisposable LogCriticalTick(this ILogger logger, string? message, params object?[] args)
    {
        return new Ticker(
        _ =>
        {
            logger.LogCritical($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogCritical($"[END]: {w.Elapsed} {message}", args);
        });
    }

    public static IDisposable LogTraceTick(this ILogger logger, string? message, params object?[] args)
    {
        return new Ticker(
        _ =>
        {
            logger.LogTrace($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogTrace($"[END]: {w.Elapsed} {message}", args);
        });
    }
}

public class Ticker : IDisposable
{
    public Stopwatch Watch { get; }

    readonly Action<Stopwatch> stop;

    public Ticker(Action<Stopwatch> start, Action<Stopwatch> stop)
    {
        this.stop = stop;
        Watch = new Stopwatch();
        Watch.Start();
        start(Watch);
    }

    public void Pause() => Watch.Stop();
    public void Resume() => Watch.Start();

    public void Dispose()
    {
        Watch.Stop();
        stop(Watch);
    }
}