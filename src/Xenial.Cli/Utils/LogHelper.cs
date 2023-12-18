﻿using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.Utils;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression")]
public static class LogHelper
{
    public static IDisposable LogInformationTick(this ILogger logger, string? message, params object?[] args)
        => new Ticker(
        _ =>
        {
            logger.LogInformation($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogInformation($"[END]: {w.Elapsed} {message}", args);
        });

    public static IDisposable LogWarningTick(this ILogger logger, string? message, params object?[] args)
        => new Ticker(
        _ =>
        {
            logger.LogWarning($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogWarning($"[END]: {w.Elapsed} {message}", args);
        });


    public static IDisposable LogErrorTick(this ILogger logger, string? message, params object?[] args)
        => new Ticker(
        _ =>
        {
            logger.LogError($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogError($"[END]: {w.Elapsed} {message}", args);
        });

    public static IDisposable LogDebugTick(this ILogger logger, string? message, params object?[] args)
        => new Ticker(
        _ =>
        {
            logger.LogDebug($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogDebug($"[END]: {w.Elapsed} {message}", args);
        });

    public static IDisposable LogCriticalTick(this ILogger logger, string? message, params object?[] args)
        => new Ticker(
        _ =>
        {
            logger.LogCritical($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogCritical($"[END]: {w.Elapsed} {message}", args);
        });

    public static IDisposable LogTraceTick(this ILogger logger, string? message, params object?[] args)
        => new Ticker(
        _ =>
        {
            logger.LogTrace($"[START]: {message}", args);
        },
        w =>
        {
            logger.LogTrace($"[END]: {w.Elapsed} {message}", args);
        });
}

public class Ticker : IDisposable
{
    public Stopwatch Watch { get; }

    private readonly Action<Stopwatch> stop;
    private bool disposedValue;

    public Ticker(Action<Stopwatch> start, Action<Stopwatch> stop)
    {
        this.stop = stop;
        Watch = new Stopwatch();
        Watch.Start();
        start(Watch);
    }

    public void Pause() => Watch.Stop();
    public void Resume() => Watch.Start();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Watch.Stop();
                stop(Watch);
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
