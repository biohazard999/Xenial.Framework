using DevExpress.ExpressApp;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
public sealed class XafDeeplinkEngine : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    public XafDeeplinkSingleInstance SingleInstance { get; }

    /// <summary>
    /// 
    /// </summary>
    public SynchronizationContext? SynchronizationContext { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="synchronizationContext"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Is disposed by owner")]
    public static XafDeeplinkEngine Create(XafApplication application, SynchronizationContext? synchronizationContext = null)
    {
        _ = application ?? throw new ArgumentNullException(nameof(application));
        return new XafDeeplinkEngine(new XafDeeplinkSingleInstance(application), synchronizationContext);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="identifier"></param>
    /// <param name="synchronizationContext"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Is disposed by owner")]
    public static XafDeeplinkEngine Create(XafApplication application, string identifier, SynchronizationContext? synchronizationContext = null)
    {
        _ = application ?? throw new ArgumentNullException(nameof(application));
        return new XafDeeplinkEngine(new XafDeeplinkSingleInstance(application, identifier), synchronizationContext);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deeplinkSingleInstance"></param>
    /// <param name="synchronizationContext"></param>
    /// <returns></returns>
    public static XafDeeplinkEngine Create(XafDeeplinkSingleInstance deeplinkSingleInstance, SynchronizationContext? synchronizationContext = null)
        => new XafDeeplinkEngine(deeplinkSingleInstance, synchronizationContext);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="singleInstance"></param>
    /// <param name="synchronizationContext"></param>
    public XafDeeplinkEngine(XafDeeplinkSingleInstance singleInstance, SynchronizationContext? synchronizationContext)
        => (SingleInstance, SynchronizationContext) = (singleInstance ?? throw new ArgumentNullException(nameof(singleInstance)), synchronizationContext ?? SynchronizationContext.Current);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="allowMultipleInstances"></param>
    /// <param name="executablePath"></param>
    /// <returns>True if the application should exit</returns>
    public async Task<bool> ListenAsync(bool allowMultipleInstances = false, string? executablePath = null)
    {
        if (!SingleInstance.IsFirstInstance)
        {
            if (ShouldPassToFirstInstance(allowMultipleInstances, executablePath))
            {
                await SingleInstance.PassArgumentsToFirstInstanceAsync().ConfigureAwait(false);
                return true;
            }
        }

        SingleInstance.ArgumentsReceived += SingleInstance_ArgumentsReceived;
        SingleInstance.ListenForArgumentsFromSuccessiveInstances();

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="allowMultipleInstances"></param>
    /// <param name="executablePath"></param>
    /// <returns>True if the application should exit</returns>
    public bool Listen(bool allowMultipleInstances = false, string? executablePath = null)
    {
        if (!SingleInstance.IsFirstInstance)
        {
            if (ShouldPassToFirstInstance(allowMultipleInstances, executablePath))
            {
                SingleInstance.PassArgumentsToFirstInstance();
                return true;
            }
        }

        SingleInstance.ArgumentsReceived += SingleInstance_ArgumentsReceived;
        SingleInstance.ListenForArgumentsFromSuccessiveInstances();

        return false;
    }

    private bool ShouldPassToFirstInstance(bool allowMultipleInstances, string? executablePath)
    {
        if (allowMultipleInstances && !string.IsNullOrEmpty(executablePath))
        {
            var arguments = SingleInstance.GetArguments();
            if (arguments.Length == 1)
            {
                var firstArgument = arguments[0];

                if (firstArgument.Equals(executablePath, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                static string? GetProcessPath()
                    =>
#if NET6_0_OR_GREATER
                    Environment.ProcessPath;
#else
                    Process.GetCurrentProcess().MainModule?.FileName;
#endif

                if (Uri.TryCreate(firstArgument, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(executablePath))
                {
                    if (uri.Scheme == "file")
                    {
                        var processPath = GetProcessPath();

                        if (executablePath!.Equals(processPath, StringComparison.Ordinal))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    var processPath = GetProcessPath();
                    if (executablePath!.Equals(processPath, StringComparison.Ordinal))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private void SingleInstance_ArgumentsReceived(object? sender, DeeplinkArgumentsReceivedEventArgs e)
    {
        //TODO: Queue up if app is not ready yet
        if (SynchronizationContext is not null)
        {
            SynchronizationContext.Post((state) =>
            {
                if (state is Tuple<Window, IList<string>> tuple)
                {
                    var (mainWindow, arguments) = tuple;
                    HandleArguments(mainWindow?.Application, arguments);
                }
            }, Tuple.Create(SingleInstance.Application.MainWindow, e.Arguments));
        }
        else
        {
            HandleArguments(SingleInstance.Application, e.Arguments);
        }

        static void HandleArguments(XafApplication? application, IList<string> arguments)
        {
            if (application is not null)
            {
                var deeplinkDispatcher = new XafDeeplinkDispatcher(application);
                deeplinkDispatcher.HandleArguments(arguments);
            }
        }
    }

    private bool disposedValue;

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                SingleInstance.ArgumentsReceived -= SingleInstance_ArgumentsReceived;
                SingleInstance.Dispose();
            }

            disposedValue = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    ~XafDeeplinkEngine() => Dispose(disposing: false);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

